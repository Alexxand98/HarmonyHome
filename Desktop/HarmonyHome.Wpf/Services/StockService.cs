using HarmonyHome.Wpf.Models.DTOs;
using HarmonyHome.Wpf.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace HarmonyHome.Wpf.Services
{
    public class StockService
    {
        private readonly IApiService _apiService;

        public StockService()
        {
            _apiService = new ApiService();
        }

        public async Task<List<StockUbicacionDTO>> GetStockAsync()
        {
            ResponseApi<List<StockUbicacionDTO>>? response = await _apiService.GetAsync<ResponseApi<List<StockUbicacionDTO>>>("api/Stock");

            if (response != null && response.IsSuccess && response.Result != null) {
                return response.Result;
            }

            return new List<StockUbicacionDTO>();
        }

        public async Task<List<StockUbicacionDTO>> GetStockTiendaAsync()
        {
            ResponseApi<List<StockUbicacionDTO>>? response = await _apiService.GetAsync<ResponseApi<List<StockUbicacionDTO>>>("api/Stock/tienda");

            if (response != null && response.IsSuccess && response.Result != null) {

                return response.Result;
            }

            return new List<StockUbicacionDTO>();
        }

        public async Task<List<StockUbicacionDTO>> GetStockAlmacenAsync()
        {
            ResponseApi<List<StockUbicacionDTO>>? response = await _apiService.GetAsync<ResponseApi<List<StockUbicacionDTO>>>("api/Stock/almacen");

            if (response != null && response.IsSuccess && response.Result != null) {
                return response.Result;
            }

            return new List<StockUbicacionDTO>();
        }
    }
}