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

        public string TipoTrazabilidad { get; set; } = string.Empty;

        public decimal PrecioVenta { get; set; }

        public bool Habilitado { get; set; }

        public bool Activo { get; set; }
    }
}