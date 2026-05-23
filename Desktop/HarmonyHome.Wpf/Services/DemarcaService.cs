using HarmonyHome.Wpf.Models.DTOs;
using HarmonyHome.Wpf.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace HarmonyHome.Wpf.Services
{
    public class DemarcaService
    {
        private readonly IApiService _apiService;

        public DemarcaService()
        {

            _apiService = new ApiService();
        }

        public async Task<string> CrearDemarcaAsync(CreateDemarcaDTO demarca)
        {
            ResponseApi<object>? response = await _apiService.PostAsync<ResponseApi<object>>("api/Demarca", demarca);

            if (response != null && response.IsSuccess){
                return "Demarca correcta";
            }


            if (response != null && response.ErrorMessages.Count > 0){
                return response.ErrorMessages[0];
            }


            return "No se pudo registrar demarca";
        }
    }
}