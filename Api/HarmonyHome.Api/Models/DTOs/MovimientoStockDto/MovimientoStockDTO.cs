using HarmonyHome.Api.Models.Enums;

namespace HarmonyHome.Api.Models.DTOs.MovimientoStockDto
{
    public class MovimientoStockDTO
    {
        public int Id { get; set; }

        public int ProductoId { get; set; }

        public string ProductoReferencia { get; set; } = string.Empty;

        public string ProductoNombre { get; set; } = string.Empty;

        public int? UbicacionOrigenId { get; set; }

        public string? UbicacionOrigenCodigo { get; set; }

        public string? UbicacionOrigenNombre { get; set; }

        public int? UbicacionDestinoId { get; set; }

        public string? UbicacionDestinoCodigo { get; set; }

        public string? UbicacionDestinoNombre { get; set; }

        public int Cantidad { get; set; }

        public DateTime Fecha { get; set; }

        public string? UsuarioId { get; set; }

        public TipoMovimiento TipoMovimiento { get; set; }

        public string TipoMovimientoNombre { get; set; } = string.Empty;

        public string? Observaciones { get; set; }
    }
}