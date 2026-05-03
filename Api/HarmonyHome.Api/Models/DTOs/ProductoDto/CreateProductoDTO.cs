using System.ComponentModel.DataAnnotations;
using HarmonyHome.Api.Models.Enums;

namespace HarmonyHome.Api.Models.DTOs
{
    public class CreateProductoDTO
    {
        [Required(ErrorMessage = "La referencia del producto es obligatoria.")]
        [MaxLength(50, ErrorMessage = "La referencia no puede superar los 50 caracteres.")]
        public string Referencia { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre del producto es obligatorio.")]
        [MaxLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "La descripción no puede superar los 500 caracteres.")]
        public string? Descripcion { get; set; }

        [Required(ErrorMessage = "La categoría del producto es obligatoria.")]
        [MaxLength(100, ErrorMessage = "La categoría no puede superar los 100 caracteres.")]
        public string Categoria { get; set; } = string.Empty;

        [Range(0, double.MaxValue, ErrorMessage = "El precio de coste no puede ser negativo.")]
        public decimal PrecioCoste { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "El precio de venta no puede ser negativo.")]
        public decimal PrecioVenta { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "El stock mínimo no puede ser negativo.")]
        public int StockMinimo { get; set; }

        [Required(ErrorMessage = "El tipo de trazabilidad es obligatorio.")]
        public TipoTrazabilidad TipoTrazabilidad { get; set; }

        public bool Habilitado { get; set; } = true;

        [MaxLength(300, ErrorMessage = "La URL de la imagen no puede superar los 300 caracteres.")]
        public string? ImagenUrl { get; set; }

        [MaxLength(500, ErrorMessage = "Las observaciones no pueden superar los 500 caracteres.")]
        public string? Observaciones { get; set; }
    }
}