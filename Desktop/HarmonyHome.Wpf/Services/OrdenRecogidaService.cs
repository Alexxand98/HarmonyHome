using HarmonyHome.Wpf.Models.DTOs;
using HarmonyHome.Wpf.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace HarmonyHome.Wpf.Services
{
    public class OrdenRecogidaService
    {
        private readonly IApiService _apiService;

        public OrdenRecogidaService()
        {
            _apiService = new ApiService();
        }

        public async Task<List<OrdenRecogidaDTO>> GetOrdenesAsync()
        {
            ResponseApi<List<OrdenRecogidaDTO>>? response = await _apiService.GetAsync<ResponseApi<List<OrdenRecogidaDTO>>>("api/OrdenRecogida");

            if (response != null && response.IsSuccess && response.Result != null) {
                return response.Result;
            }

            return new List<OrdenRecogidaDTO>();
        }

        public async Task<List<OrdenRecogidaDTO>> GetPendientesAsync()
        {
            ResponseApi<List<OrdenRecogidaDTO>>? response = await _apiService.GetAsync<ResponseApi<List<OrdenRecogidaDTO>>>("api/OrdenRecogida/pendientes");

            if (response != null && response.IsSuccess && response.Result != null){
                return response.Result;
            }

            return new List<OrdenRecogidaDTO>();
        }

        public async Task<string> AsignarAsync(int id)
        {
            ResponseApi<object>? response = await _apiService.PatchAsync<ResponseApi<object>>($"api/OrdenRecogida/{id}/asignar");

            if (response != null && response.IsSuccess){
                return "Orden asignada";
            }

            if (response != null && response.ErrorMessages.Count > 0){
                return response.ErrorMessages[0];
            }

            return "No se pudo asignar";
        }

        public async Task<string> IniciarPreparacionAsync(int id)
        {
            ResponseApi<object>? response = await _apiService.PatchAsync<ResponseApi<object>>($"api/OrdenRecogida/{id}/preparacion");

            if (response != null && response.IsSuccess) {
                return "Orden en preparación";
            }

            if (response != null && response.ErrorMessages.Count > 0){
                return response.ErrorMessages[0];
            }

            return "No se pudo iniciar";
        }

        public async Task<string> FinalizarAsync(int id)
        {
            ResponseApi<object>? response = await _apiService.PatchAsync<ResponseApi<object>>($"api/OrdenRecogida/{id}/finalizar");

            if (response != null && response.IsSuccess){
                return "Orden finalizada";
            }

            if (response != null && response.ErrorMessages.Count > 0) {
                return response.ErrorMessages[0];
            }

            return "No se pudo finalizar";
        }
    }
}