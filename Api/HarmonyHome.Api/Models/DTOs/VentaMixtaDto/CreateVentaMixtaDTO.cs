using System.ComponentModel.DataAnnotations;

namespace HarmonyHome.Api.Models.DTOs.VentaMixtaDto
{
    public class CreateVentaMixtaDTO
    {
        [Required(ErrorMessage = "El cliente es obligatorio.")]
        public int ClienteId { get; set; }

        [Required(ErrorMessage = "Debe indicar al menos una línea de venta")]
        [MinLength(1, ErrorMessage = "Debe indicar al menos una línea de venta")]
        public List<CreateLineaVentaMixtaDTO> Lineas { get; set; } = new();

        [MaxLength(500, ErrorMessage = "Las observaciones no pueden superar los 500 caracteres")]
        public string? Observaciones { get; set; }
    }

    public class CreateLineaVentaMixtaDTO
    {
        [Required(ErrorMessage = "El producto es obligatorio")]
        public int ProductoId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor que cero")]
        public int Cantidad { get; set; }
    }
}