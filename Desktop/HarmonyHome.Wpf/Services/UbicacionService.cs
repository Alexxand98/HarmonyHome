using HarmonyHome.Wpf.Models.DTOs;
using HarmonyHome.Wpf.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace HarmonyHome.Wpf.Services
{
    public class UbicacionService
    {
        private readonly IApiService _apiService;

        public UbicacionService()
        {

            _apiService = new ApiService();
        }

        public async Task<List<UbicacionDTO>> GetUbicacionesAsync()
        {

            ResponseApi<List<UbicacionDTO>>? response = await _apiService.GetAsync<ResponseApi<List<UbicacionDTO>>>("api/Ubicacion");

            if (response != null && response.IsSuccess && response.Result != null) {
                return response.Result;
            }

            return new List<UbicacionDTO>();
        }

        public async Task<UbicacionDTO?> GetUbicacionByIdAsync(int id)
        {
            ResponseApi<UbicacionDTO>? response = await _apiService.GetAsync<ResponseApi<UbicacionDTO>>($"api/Ubicacion/{id}");

            if (response != null && response.IsSuccess)  {
                return response.Result;
            }

            return null;
        }

        public async Task<bool> CrearUbicacionAsync(CreateUbicacionDTO ubicacion)
        {
            ResponseApi<UbicacionDTO>? response = await _apiService.PostAsync<ResponseApi<UbicacionDTO>>("api/Ubicacion", ubicacion);

            return response != null && response.IsSuccess;
        }

        public async Task<bool> ActualizarUbicacionAsync(int id, UpdateUbicacionDTO ubicacion)
        {
            ResponseApi<UbicacionDTO>? response = await _apiService.PutAsync<ResponseApi<UbicacionDTO>>($"api/Ubicacion/{id}", ubicacion);

            return response != null && response.IsSuccess;
        }

        public async Task<string> EliminarUbicacionAsync(int id)

        {
            ResponseApi<string>? response = await _apiService.DeleteAsync<ResponseApi<string>>($"api/Ubicacion/{id}");

            if (response != null && response.IsSuccess && response.Result != null)  {
                return response.Result;
            }

            if (response != null && response.ErrorMessages.Count > 0) {
                return response.ErrorMessages[0];
            }

            return "No se pudo eliminar la ubicacion.";
        }

        public async Task<List<UbicacionDTO>> GetUbicacionesPorTipoAsync(int tipoUbicacion)
        {
            ResponseApi<List<UbicacionDTO>>? response = await _apiService.GetAsync<ResponseApi<List<UbicacionDTO>>>($"api/Ubicacion/tipo/{tipoUbicacion}");

            if (response != null && response.IsSuccess && response.Result != null) {

                return response.Result;
            }

            return new List<UbicacionDTO>();
        }
    }
}