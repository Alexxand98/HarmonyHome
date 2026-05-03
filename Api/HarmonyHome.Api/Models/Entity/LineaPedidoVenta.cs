using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HarmonyHome.Api.Models.Entity
{
    public class LineaPedidoVenta
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El pedido de venta es obligatorio.")]
        public int PedidoVentaId { get; set; }

        [ForeignKey(nameof(PedidoVentaId))]
        public PedidoVenta? PedidoVenta { get; set; }

        [Required(ErrorMessage = "El producto es obligatorio.")]
        public int ProductoId { get; set; }

        [ForeignKey(nameof(ProductoId))]
        public Producto? Producto { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor que cero.")]
        public int Cantidad { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue, ErrorMessage = "El precio unitario no puede ser negativo.")]
        public decimal PrecioUnitario { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue, ErrorMessage = "El subtotal no puede ser negativo.")]
        public decimal Subtotal { get; set; }
    }
}