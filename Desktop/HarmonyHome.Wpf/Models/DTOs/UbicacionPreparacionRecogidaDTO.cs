using System;
using System.Collections.Generic;
using System.Text;

namespace HarmonyHome.Wpf.Models.DTOs
{
    public class UbicacionPreparacionRecogidaDTO
    {
        public int UbicacionId { get; set; }

        public string UbicacionCodigo { get; set; } = string.Empty;

        public string UbicacionNombre { get; set; } = string.Empty;

        public int CantidadDisponible { get; set; }

        public int CantidadARecoger { get; set; }
    }
}