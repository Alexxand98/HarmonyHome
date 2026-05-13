using HarmonyHome.Wpf.Models.DTOs;
using HarmonyHome.Wpf.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace HarmonyHome.Wpf.Services
{
    public class UbicacionService : IUbicacionService
    {

        private readonly IApiService _apiService;

        public UbicacionService()
        {

            _apiService = new ApiService();
        }

        public async Task<List<UbicacionDTO>> GetUbicacionesAsync()
        {

            ResponseApi<List<UbicacionDTO>>? response = await _apiService.GetAsync<ResponseApi<List<UbicacionDTO>>>("api/Ubicacion");

            if (response == null){

                return new List<UbicacionDTO>();
            }

            if (!response.IsSuccess || response.Result == null) {


                return new List<UbicacionDTO>();
            }


            return response.Result;
        }


    }
}