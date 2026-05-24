using HarmonyHome.Api.Models.DTOs;
using HarmonyHome.Api.Models.Enums;

namespace HarmonyHome.Api.Models.DTOs
{
    public class OrdenRecogidaDTO
    {
        public int Id { get; set; }

        public int PedidoVentaId { get; set; }

        public DateTime FechaCreacion { get; set; }

        public EstadoOrden Estado { get; set; }

        public string EstadoNombre { get; set; } = string.Empty;

        public string? UsuarioAsignadoId { get; set; }

        public string? UsuarioAsignadoUserName { get; set; }

        public string? UsuarioAsignadoEmail { get; set; }

        public string? Observaciones { get; set; }

        public int ClienteId { get; set; }

        public string ClienteNombreCompleto { get; set; } = string.Empty;

        public decimal TotalPedido { get; set; }

        public List<LineaPedidoClienteDTO> LineasPedido { get; set; } = new List<LineaPedidoClienteDTO>();
    }
}