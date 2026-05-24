using HarmonyHome.Wpf.Models.DTOs;
using HarmonyHome.Wpf.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace HarmonyHome.Wpf.Services
{
    public class OrdenReposicionService
    {
        private readonly IApiService _apiService;

        public OrdenReposicionService()
        {
            _apiService = new ApiService();
        }

        public async Task<List<OrdenReposicionDTO>> GetOrdenesAsync()
        {
            ResponseApi<List<OrdenReposicionDTO>>? response = await _apiService.GetAsync<ResponseApi<List<OrdenReposicionDTO>>>("api/OrdenReposicion");

            if (response != null && response.IsSuccess && response.Result != null){

                return response.Result;
            }

            return new List<OrdenReposicionDTO>();
        }

        public async Task<List<OrdenReposicionDTO>> GetPendientesAsync()
        {
            ResponseApi<List<OrdenReposicionDTO>>? response = await _apiService.GetAsync<ResponseApi<List<OrdenReposicionDTO>>>("api/OrdenReposicion/pendientes");

            if (response != null && response.IsSuccess && response.Result != null){

                return response.Result;
            }

            return new List<OrdenReposicionDTO>();
        }

        public async Task<OrdenReposicionDTO?> GetOrdenByIdAsync(int id)
        {
            ResponseApi<OrdenReposicionDTO>? response = await _apiService.GetAsync<ResponseApi<OrdenReposicionDTO>>($"api/OrdenReposicion/{id}");

            if (response != null && response.IsSuccess){

                return response.Result;
            }

            return null;
        }

        public async Task<string> AsignarAsync(int id)
        {
            ResponseApi<object>? response = await _apiService.PatchAsync<ResponseApi<object>>($"api/OrdenReposicion/{id}/asignar");

            if (response != null && response.IsSuccess){

                return "Reposicion asignada correctamente";
            }

            if (response != null && response.ErrorMessages.Count > 0){

                return response.ErrorMessages[0];
            }

            return "No se pudo asignar la reposicion";
        }

        public async Task<string> FinalizarAsync(int id)
        {
            var data = new
            {
                observaciones = "Reposición finalizada correctamente desde WPF"
            };

            ResponseApi<object>? response = await _apiService.PatchAsync<ResponseApi<object>>($"api/OrdenReposicion/{id}/finalizar", data);

            if (response != null && response.IsSuccess) {

                return "Reposición finalizada correctamente";
            }

            if (response != null && response.ErrorMessages.Count > 0){

                return response.ErrorMessages[0];
            }

            return "No se pudo finalizar la reposición";
        }
    }
}