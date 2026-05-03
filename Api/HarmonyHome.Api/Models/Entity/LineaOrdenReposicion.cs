using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HarmonyHome.Api.Models.Entity
{
    public class LineaOrdenReposicion
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "La orden de reposición es obligatoria.")]
        public int OrdenReposicionId { get; set; }

        [ForeignKey(nameof(OrdenReposicionId))]
        public OrdenReposicion? OrdenReposicion { get; set; }

        [Required(ErrorMessage = "El producto es obligatorio.")]
        public int ProductoId { get; set; }

        [ForeignKey(nameof(ProductoId))]
        public Producto? Producto { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "La cantidad solicitada debe ser mayor que cero.")]
        public int CantidadSolicitada { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "La cantidad preparada no puede ser negativa.")]
        public int CantidadPreparada { get; set; }
    }
}