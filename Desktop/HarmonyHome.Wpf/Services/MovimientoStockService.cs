using HarmonyHome.Wpf.Models.DTOs;
using HarmonyHome.Wpf.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace HarmonyHome.Wpf.Services
{
    public class MovimientoStockService
    {
        private readonly IApiService _apiService;

        public MovimientoStockService()
        {
            _apiService = new ApiService();
        }

        public async Task<List<MovimientoStockDTO>> GetMovimientosAsync()
        {
            ResponseApi<List<MovimientoStockDTO>>? response = await _apiService.GetAsync<ResponseApi<List<MovimientoStockDTO>>>("api/MovimientoStock");

            if (response != null && response.IsSuccess && response.Result != null) {
                return response.Result;
            }

            return new List<MovimientoStockDTO>();
        }

        public async Task<List<MovimientoStockDTO>> GetMovimientosPorProductoAsync(int productoId)
        {
            ResponseApi<List<MovimientoStockDTO>>? response = await _apiService.GetAsync<ResponseApi<List<MovimientoStockDTO>>>($"api/MovimientoStock/producto/{productoId}");

            if (response != null && response.IsSuccess && response.Result != null) {

                return response.Result;
            }

            return new List<MovimientoStockDTO>();
        }

        public async Task<List<MovimientoStockDTO>> GetMovimientosPorUbicacionAsync(int ubicacionId)
        {
            ResponseApi<List<MovimientoStockDTO>>? response = await _apiService.GetAsync<ResponseApi<List<MovimientoStockDTO>>>($"api/MovimientoStock/ubicacion/{ubicacionId}");

            if (response != null && response.IsSuccess && response.Result != null){

                return response.Result;
            }

            return new List<MovimientoStockDTO>();
        }
    }
}