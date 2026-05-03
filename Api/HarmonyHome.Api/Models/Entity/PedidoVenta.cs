using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HarmonyHome.Api.Models.Enums;

namespace HarmonyHome.Api.Models.Entity
{
    public class PedidoVenta
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El cliente es obligatorio.")]
        public int ClienteId { get; set; }

        [ForeignKey(nameof(ClienteId))]
        public Cliente? Cliente { get; set; }

        [MaxLength(450)]
        public string? UsuarioId { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        public EstadoPedido Estado { get; set; } = EstadoPedido.Pendiente;

        public TipoPedidoVenta TipoPedidoVenta { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue, ErrorMessage = "El total no puede ser negativo.")]
        public decimal Total { get; set; }

        [MaxLength(500, ErrorMessage = "Las observaciones no pueden superar los 500 caracteres.")]
        public string? Observaciones { get; set; }

        public ICollection<LineaPedidoVenta> Lineas { get; set; } = new List<LineaPedidoVenta>();
    }
}