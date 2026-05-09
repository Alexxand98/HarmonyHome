using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HarmonyHome.Api.Models.DTOs;
using HarmonyHome.Api.Models.Entity;
using HarmonyHome.Api.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace HarmonyHome.Api.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public UserRepository(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _configuration = configuration;
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
    }
}