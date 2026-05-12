using System;
using System.Collections.Generic;
using System.Text;

namespace HarmonyHome.Wpf.Models.DTOs
{
    public class ProductoDTO
    {
        public int Id { get; set; }

        public string Referencia { get; set; } = string.Empty;

        public string Nombre { get; set; } = string.Empty;

        public string Descripcion { get; set; } = string.Empty;

        public string Categoria { get; set; } = string.Empty;

        public decimal PrecioCoste { get; set; }

        public decimal PrecioVenta { get; set; }

        public int StockMinimo { get; set; }

        public int TipoTrazabilidad { get; set; }

        public string TipoTrazabilidadNombre { get; set; } = string.Empty;

        public bool Habilitado { get; set; }

        public bool Activo { get; set; }

        public DateTime FechaAlta { get; set; }

        public string? ImagenUrl { get; set; }

        public string Observaciones { get; set; } = string.Empty;
    }
}