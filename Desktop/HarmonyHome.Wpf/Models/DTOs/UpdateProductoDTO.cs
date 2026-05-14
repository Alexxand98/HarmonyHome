using System;
using System.Collections.Generic;
using System.Text;

namespace HarmonyHome.Wpf.Models.DTOs
{
    public class UpdateProductoDTO : CreateProductoDTO
    {
        public bool Activo { get; set; } = true;
    }
}