using System;
using System.Collections.Generic;
using System.Text;

namespace HarmonyHome.Wpf.Models.DTOs
{
    public class ProductoBajoStockDTO
    {
        public int ProductoId { get; set; }

        public string Referencia { get; set; } = string.Empty;

        public string Nombre { get; set; } = string.Empty;

        public string Categoria { get; set; } = string.Empty;

        public string ImagenUrl { get; set; } = string.Empty;

        public int StockMinimo { get; set; }

        public int StockTienda { get; set; }

        public int StockAlmacen { get; set; }

        public int StockTotal { get; set; }

        public int StockEvaluado { get; set; }

        public string TipoEvaluacion { get; set; } = string.Empty;
    }
}