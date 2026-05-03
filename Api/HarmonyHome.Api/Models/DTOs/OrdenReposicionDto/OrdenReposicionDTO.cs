using HarmonyHome.Api.Models.Enums;

namespace HarmonyHome.Api.Models.DTOs
{
    public class OrdenReposicionDTO
    {
        public int Id { get; set; }

        public DateTime FechaSolicitud { get; set; }

        public EstadoOrden Estado { get; set; }

        public string EstadoNombre { get; set; } = string.Empty;

        public string? UsuarioSolicitanteId { get; set; }

        public string? UsuarioPreparadorId { get; set; }

        public string? Observaciones { get; set; }

        public List<LineaOrdenReposicionDTO> Lineas { get; set; } = new List<LineaOrdenReposicionDTO>();
    }
}