using HarmonyHome.Api.Models.Enums;

namespace HarmonyHome.Api.Models.DTOs
{
    public class DemarcaDTO
    {
        public int MovimientoStockId { get; set; }

        public int ProductoId { get; set; }

        public string ProductoReferencia { get; set; } = string.Empty;

        public string ProductoNombre { get; set; } = string.Empty;

        public int UbicacionId { get; set; }

        public string UbicacionCodigo { get; set; } = string.Empty;

        public string UbicacionNombre { get; set; } = string.Empty;

        public int Cantidad { get; set; }

        public TipoMovimiento TipoMovimiento { get; set; }

        public string TipoMovimientoNombre { get; set; } = string.Empty;

        public string? UsuarioId { get; set; }

        public DateTime Fecha { get; set; }

        public string? Motivo { get; set; }
    }
}