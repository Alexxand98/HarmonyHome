namespace HarmonyHome.Api.Models.DTOs.PreparacionRecogidaDto
{
    public class PreparacionRecogidaDTO
    {
        public int OrdenRecogidaId { get; set; }

        public int PedidoVentaId { get; set; }

        public string EstadoOrden { get; set; } = string.Empty;

        public string ClienteNombreCompleto { get; set; } = string.Empty;

        public List<LineaPreparacionRecogidaDTO> Lineas { get; set; } = new();
    }

    public class LineaPreparacionRecogidaDTO
    {
        public int ProductoId { get; set; }

        public string ProductoReferencia { get; set; } = string.Empty;

        public string ProductoNombre { get; set; } = string.Empty;

        public int CantidadSolicitada { get; set; }

        public List<UbicacionPreparacionRecogidaDTO> Ubicaciones { get; set; } = new();
    }

    public class UbicacionPreparacionRecogidaDTO
    {
        public int UbicacionId { get; set; }

        public string UbicacionCodigo { get; set; } = string.Empty;

        public string UbicacionNombre { get; set; } = string.Empty;

        public int CantidadDisponible { get; set; }

        public int CantidadARecoger { get; set; }
    }
}