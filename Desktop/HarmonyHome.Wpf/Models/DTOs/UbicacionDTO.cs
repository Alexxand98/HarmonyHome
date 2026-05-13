using System;
using System.Collections.Generic;
using System.Text;

namespace HarmonyHome.Wpf.Models.DTOs
{
    public class UbicacionDTO
    {
        public int Id { get; set; }

        public string Codigo { get; set; } = string.Empty;

        public string Nombre { get; set; } = string.Empty;

        public int TipoUbicacion { get; set; }

        public string TipoUbicacionNombre { get; set; } = string.Empty;

        public bool Activa { get; set; }
    }
}