using System.ComponentModel.DataAnnotations;

namespace HarmonyHome.Api.Models.DTOs.StockDto
{
    public class MoveStockDTO
    {
        [Required(ErrorMessage = "El producto es obligatorio.")]
        public int ProductoId { get; set; }

        [Required(ErrorMessage = "La ubicacion de origen es obligatoria.")]
        public int UbicacionOrigenId { get; set; }

        [Required(ErrorMessage = "La ubicacion de destino es obligatoria.")]
        public int UbicacionDestinoId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor que cero.")]
        public int Cantidad { get; set; }

        [MaxLength(500, ErrorMessage = "Las observaciones no pueden superar los 500 caracteres.")]
        public string? Observaciones { get; set; }
    }
}