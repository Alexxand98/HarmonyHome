using HarmonyHome.Wpf.Helpers;
using HarmonyHome.Wpf.Models.DTOs;
using HarmonyHome.Wpf.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace HarmonyHome.Wpf.Services
{
    public class AuthService : IAuthService
    {

        private readonly IApiService _apiService;

        public AuthService()
        {
            _apiService = new ApiService();
        }

        public async Task<bool> LoginAsync(string email, string password)
        {

            LoginRequestDTO loginRequest = new LoginRequestDTO
            {
                Email = email,

                Password = password
            };


            ResponseApi<LoginResponseDTO>? response = await _apiService.PostAsync<ResponseApi<LoginResponseDTO>>("api/Auth/login", loginRequest);


            if (response == null){
                return false;

            }

            if (!response.IsSuccess || response.Result == null){
                return false;
            }


            LoginResponseDTO loginData = response.Result;


            if (!CanAccessWpf(loginData.Role)){
                return false;
            }


            SessionManager.SetSession(loginData.Token, loginData.Email, loginData.UserName, loginData.Role, loginData.Expiration);

            return true;
        }

        public bool CanAccessWpf(string rol)
        {
            if (string.IsNullOrWhiteSpace(rol)) {
                return false;

            }


            if (rol == "Logistico" || rol == "SupervisorLogistico" || rol == "Administrador") {
                return true;

            }

            return false;
        }

        public void Logout()
        {

            SessionManager.ClearSession();
        }
    
    
    
    }
}