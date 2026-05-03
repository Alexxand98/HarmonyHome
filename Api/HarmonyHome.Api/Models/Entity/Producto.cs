using System.ComponentModel.DataAnnotations;

namespace HarmonyHome.Api.Models.Entity
{
    public class Producto
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Referencia { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Descripcion { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Precio { get; set; }

        public bool Habilitado { get; set; } = true;
    }
}