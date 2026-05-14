using System;
using System.Collections.Generic;
using System.Text;

namespace HarmonyHome.Wpf.Models.DTOs
{
    public class CreateProductoDTO
    {
        public string Referencia { get; set; } = string.Empty;

        public string Nombre { get; set; } = string.Empty;

        public string? Descripcion { get; set; }

        public string Categoria { get; set; } = string.Empty;

        public decimal PrecioCoste { get; set; }

        public decimal PrecioVenta { get; set; }

        public int StockMinimo { get; set; }

        public int TipoTrazabilidad { get; set; }

        public bool Habilitado { get; set; } = true;

        public string? ImagenUrl { get; set; }

        public string? Observaciones { get; set; }
    }
}