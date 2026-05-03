using HarmonyHome.Api.Models.Enums;

namespace HarmonyHome.Api.Models.DTOs
{
    public class PedidoClienteDTO
    {
        public int Id { get; set; }

        public int ClienteId { get; set; }

        public string ClienteNombreCompleto { get; set; } = string.Empty;

        public string? UsuarioId { get; set; }

        public DateTime FechaCreacion { get; set; }

        public EstadoPedido Estado { get; set; }

        public string EstadoNombre { get; set; } = string.Empty;

        public TipoPedidoVenta TipoPedidoVenta { get; set; }

        public string TipoPedidoVentaNombre { get; set; } = string.Empty;

        public decimal Total { get; set; }

        public string? Observaciones { get; set; }

        public int? OrdenRecogidaId { get; set; }

        public string? EstadoOrdenRecogidaNombre { get; set; }

        public List<LineaPedidoClienteDTO> Lineas { get; set; } = new List<LineaPedidoClienteDTO>();
    }
}