using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HarmonyHome.Api.Models.Enums;

namespace HarmonyHome.Api.Models.Entity
{
    public class OrdenRecogida
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El pedido de venta es obligatorio.")]
        public int PedidoVentaId { get; set; }

        [ForeignKey(nameof(PedidoVentaId))]
        public PedidoVenta? PedidoVenta { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        public EstadoOrden Estado { get; set; } = EstadoOrden.Pendiente;

        [MaxLength(450)]
        public string? UsuarioAsignadoId { get; set; }

        [MaxLength(500, ErrorMessage = "Las observaciones no pueden superar los 500 caracteres.")]
        public string? Observaciones { get; set; }
    }
}