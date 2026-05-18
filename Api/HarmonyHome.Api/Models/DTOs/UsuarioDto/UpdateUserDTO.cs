using System.ComponentModel.DataAnnotations;

namespace HarmonyHome.Api.Models.DTOs.UsuarioDto
{
    public class UpdateUserDTO
    {
        [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
        [MaxLength(100, ErrorMessage = "El nombre de usuario no puede superar los 100 caracteres.")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El email es obligatorio.")]
        [EmailAddress(ErrorMessage = "El formato del email no es válido.")]
        [MaxLength(150, ErrorMessage = "El email no puede superar los 150 caracteres.")]
        public string Email { get; set; } = string.Empty;

        [MaxLength(150, ErrorMessage = "El nombre completo no puede superar los 150 caracteres.")]
        public string? NombreCompleto { get; set; }
    }
}