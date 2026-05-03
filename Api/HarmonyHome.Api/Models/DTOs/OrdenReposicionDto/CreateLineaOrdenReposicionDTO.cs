using System.ComponentModel.DataAnnotations;

namespace HarmonyHome.Api.Models.DTOs
{
    public class CreateLineaOrdenReposicionDTO
    {
        [Required(ErrorMessage = "El producto es obligatorio.")]
        public int ProductoId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "La cantidad solicitada debe ser mayor que cero.")]
        public int CantidadSolicitada { get; set; }
    }
}