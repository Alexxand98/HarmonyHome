using System;
using System.Collections.Generic;
using System.Text;

namespace HarmonyHome.Wpf.Models.DTOs
{
    public class CreateUbicacionDTO
    {
        public string Codigo { get; set; } = string.Empty;

        public string Nombre { get; set; } = string.Empty;

        public int TipoUbicacion { get; set; }

        public bool Activa { get; set; } = true;
    }
}