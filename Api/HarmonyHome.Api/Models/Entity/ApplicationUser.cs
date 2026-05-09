using Microsoft.AspNetCore.Identity;

namespace HarmonyHome.Api.Models.Entity
{
    public class ApplicationUser : IdentityUser
    {
        public string? NombreCompleto { get; set; }

        public bool Activo { get; set; } = true;

        public DateTime FechaAlta { get; set; } = DateTime.UtcNow;
    }
}