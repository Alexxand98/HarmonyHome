using HarmonyHome.Api.Data;
using HarmonyHome.Api.Models.DTOs;
using HarmonyHome.Api.Models.DTOs.UsuarioDto;
using HarmonyHome.Api.Models.Entity;
using HarmonyHome.Api.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HarmonyHome.Api.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;

        public UserRepository(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _context = context;
        }

        public bool IsUniqueUser(string email)
        {
            var user = _userManager.Users.FirstOrDefault(u => u.Email == email);
            return user == null;
        }

        public async Task<UserLoginResponseDTO?> Login(UserLoginDTO userLoginDTO)
        {
            var user = await _userManager.FindByEmailAsync(userLoginDTO.Email);

            if (user == null || !user.Activo)
            {
                return null;
            }

            var result = await _signInManager.CheckPasswordSignInAsync(
                user,
                userLoginDTO.Password,
                false
            );

            if (!result.Succeeded)
            {
                return null;
            }

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? string.Empty;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = _configuration.GetValue<string>("ApiSettings:Secret");

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                    new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                    new Claim(ClaimTypes.Role, role)
                }),
                Expires = DateTime.UtcNow.AddHours(8),
                Issuer = _configuration.GetValue<string>("ApiSettings:Issuer"),
                Audience = _configuration.GetValue<string>("ApiSettings:Audience"),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key!)),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new UserLoginResponseDTO
            {
                Id = user.Id,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                Role = role,
                Token = tokenHandler.WriteToken(token),
                Expiration = tokenDescriptor.Expires.Value
            };
        }

        public async Task<UserDTO?> Register(UserRegisterDTO userRegisterDTO)
        {
            if (!IsUniqueUser(userRegisterDTO.Email))
            {
                return null;
            }

            if (!await _roleManager.RoleExistsAsync(userRegisterDTO.Role))
            {
                await _roleManager.CreateAsync(new IdentityRole(userRegisterDTO.Role));
            }

            var user = new ApplicationUser
            {
                UserName = userRegisterDTO.UserName,
                Email = userRegisterDTO.Email,
                NombreCompleto = userRegisterDTO.UserName,
                Activo = true,
                FechaAlta = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, userRegisterDTO.Password);

            if (!result.Succeeded)
            {
                return null;
            }

            await _userManager.AddToRoleAsync(user, userRegisterDTO.Role);

            return new UserDTO
            {
                Id = user.Id,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                Role = userRegisterDTO.Role
            };
        }

        public async Task<UserDTO?> GetUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return null;
            }

            var roles = await _userManager.GetRolesAsync(user);

            return new UserDTO
            {
                Id = user.Id,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                Role = roles.FirstOrDefault() ?? string.Empty
            };
        }

        public Task<List<UserDTO>> GetUsers()
        {
            var users = _userManager.Users
                .Select(u => new UserDTO
                {
                    Id = u.Id,
                    UserName = u.UserName ?? string.Empty,
                    Email = u.Email ?? string.Empty,
                    Role = string.Empty
                })
                .ToList();

            return Task.FromResult(users);
        }



        public async Task<List<UserDTO>> GetLogisticos()
        {
            var users = await _userManager.GetUsersInRoleAsync("Logistico");

            return users.Select(u => new UserDTO
            {
                Id = u.Id,
                UserName = u.UserName ?? string.Empty,
                Email = u.Email ?? string.Empty,
                NombreCompleto = u.NombreCompleto,
                Role = "Logistico",
                Activo = u.Activo,
                FechaAlta = u.FechaAlta

            }).ToList();
        }

        public async Task<UserDTO?> CreateLogistico(CreateLogisticoDTO createLogisticoDTO)
        {
            if (!IsUniqueUser(createLogisticoDTO.Email)){
                return null;
            }

            if (!await _roleManager.RoleExistsAsync("Logistico")) {

                await _roleManager.CreateAsync(new IdentityRole("Logistico"));
            }

            var user = new ApplicationUser
            {
                UserName = createLogisticoDTO.UserName,
                Email = createLogisticoDTO.Email,
                NombreCompleto = createLogisticoDTO.NombreCompleto ?? createLogisticoDTO.UserName,
                Activo = true,
                FechaAlta = DateTime.UtcNow,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, createLogisticoDTO.Password);

            if (!result.Succeeded)
            {
                return null;
            }

            await _userManager.AddToRoleAsync(user, "Logistico");

            return new UserDTO
            {
                Id = user.Id,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                NombreCompleto = user.NombreCompleto,
                Role = "Logistico",
                Activo = user.Activo,
                FechaAlta = user.FechaAlta
            };
        }

        public async Task<UserDTO?> UpdateUser(string id, UpdateUserDTO updateUserDTO)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)  {

                return null;
            }

            var emailOwner = await _userManager.FindByEmailAsync(updateUserDTO.Email);

            if (emailOwner != null && emailOwner.Id != id)   {

                return null;
            }

            user.UserName = updateUserDTO.UserName;

            user.Email = updateUserDTO.Email;

            user.NombreCompleto = updateUserDTO.NombreCompleto;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded) {

                return null;
            }

            var roles = await _userManager.GetRolesAsync(user);

            return new UserDTO
            {
                Id = user.Id,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                NombreCompleto = user.NombreCompleto,
                Role = roles.FirstOrDefault() ?? string.Empty,
                Activo = user.Activo,
                FechaAlta = user.FechaAlta
            };
        }

        public async Task<bool> ActivarUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null) {

                return false;
            }

            user.Activo = true;

            var result = await _userManager.UpdateAsync(user);

            return result.Succeeded;
        }

        public async Task<bool> DesactivarUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null) {

                return false;
            }

            user.Activo = false;

            var result = await _userManager.UpdateAsync(user);

            return result.Succeeded;
        }

        public async Task<string?> DeleteUser(string id)
        {
            var usuario = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

            if (usuario == null)
            {
                return null;
            }

            var tieneRelaciones =
                await _context.OrdenesRecogida.AnyAsync(o => o.UsuarioAsignadoId == id) ||
                await _context.OrdenesReposicion.AnyAsync(o =>
                    o.UsuarioSolicitanteId == id ||
                    o.UsuarioPreparadorId == id) ||
                await _context.MovimientosStock.AnyAsync(m => m.UsuarioId == id) ||
                await _context.PedidosVenta.AnyAsync(p => p.UsuarioId == id);

            if (!tieneRelaciones)
            {
                var resultadoDelete = await _userManager.DeleteAsync(usuario);

                if (!resultadoDelete.Succeeded)
                {
                    return "No se pudo eliminar el usuario.";
                }

                return "Usuario eliminado correctamente.";
            }

            usuario.Activo = false;

            var resultadoUpdate = await _userManager.UpdateAsync(usuario);

            if (!resultadoUpdate.Succeeded)
            {
                return "No se pudo desactivar el usuario.";
            }

            return "El usuario tiene datos asociados. Baja lógica realizada correctamente.";
        }
    }
}