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

        public string? UsuarioSolicitanteUserName { get; set; }

        public string? UsuarioSolicitanteEmail { get; set; }

        public string? UsuarioPreparadorUserName { get; set; }

        public string? UsuarioPreparadorEmail { get; set; }

        public string Observaciones { get; set; } = string.Empty;

        public List<LineaOrdenReposicionDTO> Lineas { get; set; } = new List<LineaOrdenReposicionDTO>();

        public string SolicitanteTexto
        {
            get
            {
                if (string.IsNullOrWhiteSpace(UsuarioSolicitanteUserName) && string.IsNullOrWhiteSpace(UsuarioSolicitanteEmail)){
                    return "Sin solicitante";
                }

                return UsuarioSolicitanteUserName + " - " + UsuarioSolicitanteEmail;
            }
        }

        public string PreparadorTexto
        {
            get
            {
                if (string.IsNullOrWhiteSpace(UsuarioPreparadorUserName) && string.IsNullOrWhiteSpace(UsuarioPreparadorEmail)) {
                    return "Sin asignar";
                }

                return UsuarioPreparadorUserName + " - " + UsuarioPreparadorEmail;
            }
        }
    }


}