using System;
using System.Collections.Generic;
using System.Text;

using System.Threading.Tasks;

namespace HarmonyHome.Wpf.Services.Interfaces
{
    public interface IAuthService
    {
        Task<bool> LoginAsync(string email, string password);

        bool CanAccessWpf(string rol);

        void Logout();
    }
}