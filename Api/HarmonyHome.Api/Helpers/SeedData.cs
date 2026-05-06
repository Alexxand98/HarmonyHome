using HarmonyHome.Api.Models.Entity;
using Microsoft.AspNetCore.Identity;

namespace HarmonyHome.Api.Helpers
{
    public static class SeedData
    {
        public static async Task InicializarRolesYUsuariosAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            string[] roles =
            {
                "Vendedor",
                "EncargadoTienda",
                "Logistico",
                "SupervisorLogistico",
                "Administrador"
            };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            await CrearUsuarioSiNoExiste(
                userManager,
                "vendedor@harmonyhome.com",
                "VendedorDemo",
                "Vendedor",
                "Password123!"
            );

            await CrearUsuarioSiNoExiste(
                userManager,
                "encargado@harmonyhome.com",
                "EncargadoDemo",
                "EncargadoTienda",
                "Password123!"
            );

            await CrearUsuarioSiNoExiste(
                userManager,
                "logistico@harmonyhome.com",
                "LogisticoDemo",
                "Logistico",
                "Password123!"
            );

            await CrearUsuarioSiNoExiste(
                userManager,
                "supervisor@harmonyhome.com",
                "SupervisorDemo",
                "SupervisorLogistico",
                "Password123!"
            );

            await CrearUsuarioSiNoExiste(
                userManager,
                "admin@harmonyhome.com",
                "AdminDemo",
                "Administrador",
                "Password123!"
            );

            await CrearUsuarioSiNoExiste(
                userManager,
                "logistico@harmonyhome.com",
                "LogisticoDemo",
                "Logistico",
                "admin?"
            );

            await CrearUsuarioSiNoExiste(
                userManager,
                "encargado@harmonyhome.com",
                "EncargadoDemo",
                "EncargadoTienda",
                "admin1?!"
            );

            await CrearUsuarioSiNoExiste(
                userManager,
                "vendedor@harmonyhome.com",
                "AdminDemo",
                "Administrador",
                "admin2?!"
            );


        }



        private static async Task CrearUsuarioSiNoExiste(
            UserManager<ApplicationUser> userManager,
            string email,
            string userName,
            string role,
            string password)
        {
            var usuario = await userManager.FindByEmailAsync(email);

            if (usuario != null)
            {
                return;
            }

            var nuevoUsuario = new ApplicationUser
            {
                UserName = userName,
                Email = email,
                NombreCompleto = userName,
                Activo = true,
                FechaAlta = DateTime.UtcNow,
                EmailConfirmed = true
            };

            var resultado = await userManager.CreateAsync(nuevoUsuario, password);

            if (resultado.Succeeded)
            {
                await userManager.AddToRoleAsync(nuevoUsuario, role);
            }
        }
    }
}