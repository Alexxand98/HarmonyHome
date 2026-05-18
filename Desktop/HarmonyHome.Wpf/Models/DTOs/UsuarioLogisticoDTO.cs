using System;
using System.Collections.Generic;
using System.Text;

namespace HarmonyHome.Wpf.Models.DTOs
{
    public class UsuarioLogisticoDTO
    {
        public string Id { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string NombreCompleto { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public bool Activo { get; set; }

        public DateTime FechaAlta { get; set; }
    }
}