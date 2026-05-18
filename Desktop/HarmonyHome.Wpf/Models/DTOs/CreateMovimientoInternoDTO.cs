using System;
using System.Collections.Generic;
using System.Text;

namespace HarmonyHome.Wpf.Models.DTOs
{
    public class CreateMovimientoInternoDTO
    {
        public int ProductoId { get; set; }

        public int UbicacionOrigenId { get; set; }

        public int UbicacionDestinoId { get; set; }

        public int Cantidad { get; set; }

        public string? Observaciones { get; set; }
    }
}