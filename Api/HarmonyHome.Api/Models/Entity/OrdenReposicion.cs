using System.ComponentModel.DataAnnotations;
using HarmonyHome.Api.Models.Enums;

namespace HarmonyHome.Api.Models.Entity
{
    public class OrdenReposicion
    {
        [Key]
        public int Id { get; set; }

        public DateTime FechaSolicitud { get; set; } = DateTime.UtcNow;

        public EstadoOrden Estado { get; set; } = EstadoOrden.Pendiente;

        [MaxLength(450)]
        public string? UsuarioSolicitanteId { get; set; }

        [MaxLength(450)]
        public string? UsuarioPreparadorId { get; set; }

        [MaxLength(500, ErrorMessage = "Las observaciones no pueden superar los 500 caracteres.")]
        public string? Observaciones { get; set; }

        public ICollection<LineaOrdenReposicion> Lineas { get; set; } = new List<LineaOrdenReposicion>();
    }
}