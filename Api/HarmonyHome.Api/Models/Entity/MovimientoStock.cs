using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HarmonyHome.Api.Models.Enums;

namespace HarmonyHome.Api.Models.Entity
{
    public class MovimientoStock
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El producto es obligatorio.")]
        public int ProductoId { get; set; }

        [ForeignKey(nameof(ProductoId))]
        public Producto? Producto { get; set; }

        public int? UbicacionOrigenId { get; set; }

        [ForeignKey(nameof(UbicacionOrigenId))]
        public Ubicacion? UbicacionOrigen { get; set; }

        public int? UbicacionDestinoId { get; set; }

        [ForeignKey(nameof(UbicacionDestinoId))]
        public Ubicacion? UbicacionDestino { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor que cero.")]
        public int Cantidad { get; set; }

        public DateTime Fecha { get; set; } = DateTime.UtcNow;

        [MaxLength(450)]
        public string? UsuarioId { get; set; }

        [Required(ErrorMessage = "El tipo de movimiento es obligatorio.")]
        public TipoMovimiento TipoMovimiento { get; set; }

        [MaxLength(500, ErrorMessage = "Las observaciones no pueden superar los 500 caracteres.")]
        public string? Observaciones { get; set; }
    }
}