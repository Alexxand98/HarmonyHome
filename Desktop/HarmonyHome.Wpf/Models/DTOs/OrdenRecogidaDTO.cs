using System;
using System.Collections.Generic;
using System.Text;

namespace HarmonyHome.Wpf.Models.DTOs
{
    public class OrdenRecogidaDTO
    {
        public int Id { get; set; }

        public int PedidoVentaId { get; set; }

        public DateTime FechaCreacion { get; set; }

        public int Estado { get; set; }

        public string EstadoNombre { get; set; } = string.Empty;

        public string? UsuarioAsignadoId { get; set; }

        public string Observaciones { get; set; } = string.Empty;

        public int ClienteId { get; set; }

        public string ClienteNombreCompleto { get; set; } = string.Empty;

        public decimal TotalPedido { get; set; }

        public List<LineaPedidoVentaDTO> LineasPedido { get; set; } = new List<LineaPedidoVentaDTO>();
    }
}