using System;
using System.Collections.Generic;
using System.Text;

namespace HarmonyHome.Wpf.Models.DTOs
{
    public class LoginResponseDTO
    {
        public string Id { get; set; } = string.Empty;


        public string Token { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public DateTime Expiration { get; set; }
    }
}