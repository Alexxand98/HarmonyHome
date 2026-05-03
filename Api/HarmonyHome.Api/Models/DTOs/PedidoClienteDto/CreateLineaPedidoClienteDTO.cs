using System.ComponentModel.DataAnnotations;

namespace HarmonyHome.Api.Models.DTOs
{
    public class CreateLineaPedidoClienteDTO
    {
        [Required(ErrorMessage = "El producto es obligatorio.")]
        public int ProductoId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor que cero.")]
        public int Cantidad { get; set; }
    }
}