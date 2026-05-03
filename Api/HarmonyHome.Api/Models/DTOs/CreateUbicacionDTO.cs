using System.ComponentModel.DataAnnotations;
using HarmonyHome.Api.Models.Enums;

namespace HarmonyHome.Api.Models.DTOs
{
    public class CreateUbicacionDTO
    {
        [Required(ErrorMessage = "El código de la ubicación es obligatorio.")]
        [MaxLength(50, ErrorMessage = "El código no puede superar los 50 caracteres.")]
        public string Codigo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre de la ubicación es obligatorio.")]
        [MaxLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El tipo de ubicación es obligatorio.")]
        public TipoUbicacion TipoUbicacion { get; set; }
    }
}