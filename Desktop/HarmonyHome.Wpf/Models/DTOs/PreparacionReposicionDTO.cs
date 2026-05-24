using System;
using System.Collections.Generic;
using System.Text;

namespace HarmonyHome.Wpf.Models.DTOs
{
    public class PreparacionReposicionDTO
    {
        public int OrdenReposicionId { get; set; }

        public string EstadoOrden { get; set; } = string.Empty;

        public List<LineaPreparacionReposicionDTO> Lineas { get; set; } = new List<LineaPreparacionReposicionDTO>();
    }
}