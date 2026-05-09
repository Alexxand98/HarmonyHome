using System;
using System.Collections.Generic;
using System.Text;

namespace HarmonyHome.Wpf.Helpers
{
    public static class SessionManager{

        public static string Token { get; private set; } = string.Empty;

        public static string Email { get; private set; } = string.Empty;

        public static string UserName { get; private set; } = string.Empty;

        public static string Rol { get; private set; } = string.Empty;

        public static DateTime Expiration { get; private set; }



        public static bool IsLoggedIn{
            get{
                return !string.IsNullOrWhiteSpace(Token) && Expiration > DateTime.Now;
            }
        }


        public static void SetSession(string token, string email, string userName, string rol, DateTime expiration){
            Token = token;
            Email = email;
            UserName = userName;
            Rol = rol;
            Expiration = expiration;
        }

        public static void ClearSession(){
            Token = string.Empty;
            Email = string.Empty;
            UserName = string.Empty;
            Rol = string.Empty;
            Expiration = DateTime.MinValue;
        }


    }
}