using System;
using System.Collections.Generic;
using System.Text;

namespace HarmonyHome.Wpf.Models.DTOs
{
    public class CreateDemarcaDTO
    {
        public int ProductoId { get; set; }

        public int UbicacionId { get; set; }

        public int Cantidad { get; set; }

        public string Motivo { get; set; } = string.Empty;
    }
}