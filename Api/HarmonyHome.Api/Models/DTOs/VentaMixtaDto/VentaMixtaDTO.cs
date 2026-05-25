namespace HarmonyHome.Api.Models.DTOs.VentaMixtaDto
{
    public class VentaMixtaDTO
    {
        public string TipoOperacion { get; set; } = string.Empty;

        public int? VentaDirectaId { get; set; }

        public int? PedidoClienteId { get; set; }

        public int? OrdenRecogidaId { get; set; }

        public decimal TotalVentaDirecta { get; set; }

        public decimal TotalPedidoCliente { get; set; }

        public decimal TotalOperacion => TotalVentaDirecta + TotalPedidoCliente;

        public List<LineaVentaMixtaResultadoDTO> Lineas { get; set; } = new();
    }

    public class LineaVentaMixtaResultadoDTO
    {
        public int ProductoId { get; set; }

        public string ProductoReferencia { get; set; } = string.Empty;

        public string ProductoNombre { get; set; } = string.Empty;

        public int CantidadSolicitada { get; set; }

        public int CantidadVentaDirecta { get; set; }

        public int CantidadPedidoCliente { get; set; }

        public decimal PrecioUnitario { get; set; }

        public decimal SubtotalVentaDirecta { get; set; }

        public decimal SubtotalPedidoCliente { get; set; }
    }
}