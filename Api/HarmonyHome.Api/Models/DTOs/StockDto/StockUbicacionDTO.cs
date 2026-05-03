using HarmonyHome.Api.Models.Enums;

namespace HarmonyHome.Api.Models.DTOs
{
    public class StockUbicacionDTO
    {
        public int Id { get; set; }

        public int ProductoId { get; set; }

        public string ProductoReferencia { get; set; } = string.Empty;

        public string ProductoNombre { get; set; } = string.Empty;

        public int UbicacionId { get; set; }

        public string UbicacionCodigo { get; set; } = string.Empty;

        public string UbicacionNombre { get; set; } = string.Empty;

        public TipoUbicacion TipoUbicacion { get; set; }

        public string TipoUbicacionNombre { get; set; } = string.Empty;

        public int Cantidad { get; set; }
    }
}