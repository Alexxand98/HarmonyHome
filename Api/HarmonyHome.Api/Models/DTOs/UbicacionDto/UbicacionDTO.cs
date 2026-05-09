using HarmonyHome.Api.Models.Enums;

namespace HarmonyHome.Api.Models.DTOs
{
    public class UbicacionDTO
    {
        public int Id { get; set; }

        public string Codigo { get; set; } = string.Empty;

        public string Nombre { get; set; } = string.Empty;

        public TipoUbicacion TipoUbicacion { get; set; }

        public string TipoUbicacionNombre { get; set; } = string.Empty;

        public bool Activa { get; set; }
    }
}