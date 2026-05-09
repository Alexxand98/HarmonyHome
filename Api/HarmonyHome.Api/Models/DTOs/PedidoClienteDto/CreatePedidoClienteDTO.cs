using System.ComponentModel.DataAnnotations;

namespace HarmonyHome.Api.Models.DTOs
{
    public class CreatePedidoClienteDTO
    {
        [Required(ErrorMessage = "El cliente es obligatorio.")]
        public int ClienteId { get; set; }

        [Required(ErrorMessage = "Debe incluir al menos una línea de pedido.")]
        [MinLength(1, ErrorMessage = "Debe incluir al menos una línea de pedido.")]
        public List<CreateLineaPedidoClienteDTO> Lineas { get; set; } = new List<CreateLineaPedidoClienteDTO>();

        [MaxLength(500, ErrorMessage = "Las observaciones no pueden superar los 500 caracteres.")]
        public string? Observaciones { get; set; }
    }
}