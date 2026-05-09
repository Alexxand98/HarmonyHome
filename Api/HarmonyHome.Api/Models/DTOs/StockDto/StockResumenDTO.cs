namespace HarmonyHome.Api.Models.DTOs
{
    public class StockResumenDTO
    {
        public int ProductoId { get; set; }

        public string ProductoReferencia { get; set; } = string.Empty;

        public string ProductoNombre { get; set; } = string.Empty;

        public int StockTienda { get; set; }

        public int StockAlmacen { get; set; }

        public int StockTotal { get; set; }
    }
}