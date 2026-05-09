using System.ComponentModel.DataAnnotations;

namespace HarmonyHome.Api.Models.DTOs
{
    public class CreateOrdenReposicionDTO
    {
        [Required(ErrorMessage = "Debe incluir al menos una línea de reposición.")]
        [MinLength(1, ErrorMessage = "Debe incluir al menos una línea de reposición.")]
        public List<CreateLineaOrdenReposicionDTO> Lineas { get; set; } = new List<CreateLineaOrdenReposicionDTO>();

        [MaxLength(500, ErrorMessage = "Las observaciones no pueden superar los 500 caracteres.")]
        public string? Observaciones { get; set; }
    }
}