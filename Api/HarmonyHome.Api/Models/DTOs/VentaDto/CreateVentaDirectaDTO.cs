using System.ComponentModel.DataAnnotations;

namespace HarmonyHome.Api.Models.DTOs
{
    public class CreateVentaDirectaDTO
    {
        [Required(ErrorMessage = "El cliente es obligatorio.")]
        public int ClienteId { get; set; }

        [Required(ErrorMessage = "Debe incluir al menos una línea de venta.")]
        [MinLength(1, ErrorMessage = "Debe incluir al menos una línea de venta.")]
        public List<CreateLineaVentaDTO> Lineas { get; set; } = new List<CreateLineaVentaDTO>();

        [MaxLength(500, ErrorMessage = "Las observaciones no pueden superar los 500 caracteres.")]
        public string? Observaciones { get; set; }
    }
}