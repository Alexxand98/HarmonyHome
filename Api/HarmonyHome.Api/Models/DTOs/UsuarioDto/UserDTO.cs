namespace HarmonyHome.Api.Models.DTOs
{
    public class UserDTO
    {
        public string Id { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string? NombreCompleto { get; set; }


        public string Role { get; set; } = string.Empty;

        public bool Activo { get; set; }

        public DateTime FechaAlta { get; set; }
    }
}