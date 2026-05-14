using System;
using System.Collections.Generic;
using System.Text;

namespace HarmonyHome.Wpf.Models.DTOs
{
    public class OrdenReposicionDTO
    {
        public int Id { get; set; }

        public DateTime FechaSolicitud { get; set; }

        public int Estado { get; set; }

        public string EstadoNombre { get; set; } = string.Empty;

        public string UsuarioSolicitanteId { get; set; } = string.Empty;

        public string? UsuarioPreparadorId { get; set; }

        public string Observaciones { get; set; } = string.Empty;

        public List<LineaOrdenReposicionDTO> Lineas { get; set; } = new List<LineaOrdenReposicionDTO>();
    }
}