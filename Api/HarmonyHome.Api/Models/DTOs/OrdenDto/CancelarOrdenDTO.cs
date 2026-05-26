using System.ComponentModel.DataAnnotations;

namespace HarmonyHome.Api.Models.DTOs.OrdenDto
{
    public class CancelarOrdenDTO
    {
        [Required(ErrorMessage = "El motivo de cancelación es obligatorio.")]
        [MaxLength(500, ErrorMessage = "El motivo no puede superar los 500 caracteres.")]
        public string MotivoCancelacion { get; set; } = string.Empty;
    }
}