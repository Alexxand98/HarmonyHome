using System.ComponentModel.DataAnnotations;

namespace HarmonyHome.Api.Models.DTOs
{
    public class CreateDemarcaDTO
    {
        [Required(ErrorMessage = "El producto es obligatorio.")]
        public int ProductoId { get; set; }

        [Required(ErrorMessage = "La ubicación es obligatoria.")]
        public int UbicacionId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor que cero.")]
        public int Cantidad { get; set; }

        [Required(ErrorMessage = "El motivo de la demarca es obligatorio.")]
        [MinLength(5, ErrorMessage = "El motivo debe tener al menos 5 caracteres.")]
        [MaxLength(500, ErrorMessage = "El motivo no puede superar los 500 caracteres.")]
        public string Motivo { get; set; } = string.Empty;
    }
}