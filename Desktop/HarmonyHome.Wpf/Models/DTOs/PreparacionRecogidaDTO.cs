using System;
using System.Collections.Generic;
using System.Text;

namespace HarmonyHome.Wpf.Models.DTOs
{
    public class PreparacionRecogidaDTO
    {
        public int OrdenRecogidaId { get; set; }

        public int PedidoVentaId { get; set; }

        public string EstadoOrden { get; set; } = string.Empty;

        public string ClienteNombreCompleto { get; set; } = string.Empty;

        public List<LineaPreparacionRecogidaDTO> Lineas { get; set; } = new List<LineaPreparacionRecogidaDTO>();
    }
}