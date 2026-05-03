using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HarmonyHome.Api.Models.Entity
{
    public class StockUbicacion
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El producto es obligatorio.")]
        public int ProductoId { get; set; }

        [ForeignKey(nameof(ProductoId))]
        public Producto? Producto { get; set; }

        [Required(ErrorMessage = "La ubicación es obligatoria.")]
        public int UbicacionId { get; set; }

        [ForeignKey(nameof(UbicacionId))]
        public Ubicacion? Ubicacion { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "La cantidad no puede ser negativa.")]
        public int Cantidad { get; set; }
    }
}