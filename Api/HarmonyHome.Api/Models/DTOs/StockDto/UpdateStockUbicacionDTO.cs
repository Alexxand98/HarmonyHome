using System.ComponentModel.DataAnnotations;

namespace HarmonyHome.Api.Models.DTOs
{
    public class UpdateStockUbicacionDTO
    {
        [Range(0, int.MaxValue, ErrorMessage = "La cantidad no puede ser negativa.")]
        public int Cantidad { get; set; }
    }
}