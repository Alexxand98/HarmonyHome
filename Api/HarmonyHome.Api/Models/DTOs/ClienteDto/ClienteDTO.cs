namespace HarmonyHome.Api.Models.DTOs
{
    public class ClienteDTO
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public string? Apellidos { get; set; }

        public string? Telefono { get; set; }

        public string? Email { get; set; }

        public string? Direccion { get; set; }

        public bool Activo { get; set; }

        public DateTime FechaAlta { get; set; }
    }
}