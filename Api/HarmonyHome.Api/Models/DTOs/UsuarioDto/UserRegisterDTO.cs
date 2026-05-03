using System.ComponentModel.DataAnnotations;

namespace HarmonyHome.Api.Models.DTOs
{
    public class UserRegisterDTO
    {
        [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
        [MaxLength(100, ErrorMessage = "El nombre de usuario no puede superar los 100 caracteres.")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El email es obligatorio.")]
        [EmailAddress(ErrorMessage = "El formato del email no es válido.")]
        [MaxLength(150, ErrorMessage = "El email no puede superar los 150 caracteres.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "El rol es obligatorio.")]
        public string Role { get; set; } = string.Empty;
    }
}