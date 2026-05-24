using System;
using System.Collections.Generic;
using System.Text;

namespace HarmonyHome.Wpf.Models.DTOs
{
    public class LineaPreparacionRecogidaDTO
    {
        public int ProductoId { get; set; }

        public string ProductoReferencia { get; set; } = string.Empty;

        public string ProductoNombre { get; set; } = string.Empty;

        public int CantidadSolicitada { get; set; }

        public List<UbicacionPreparacionRecogidaDTO> Ubicaciones { get; set; } = new List<UbicacionPreparacionRecogidaDTO>();
    }
}