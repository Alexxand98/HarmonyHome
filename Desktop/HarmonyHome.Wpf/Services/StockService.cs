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




        public async Task<string> MoverStockAsync(CreateMovimientoInternoDTO movimiento)
        {
            ResponseApi<string>? response = await _apiService.PostAsync<ResponseApi<string>>("api/Stock/mover", movimiento);

            if (response != null && response.IsSuccess && response.Result != null) {
                return response.Result;
            }

            if (response != null && response.ErrorMessages.Count > 0){
                return response.ErrorMessages[0];
            }

            return "No se pudo mover el stock";
        }



        public async Task<string> CrearStockAsync(CreateStockUbicacionDTO stock)
        {
            ResponseApi<StockUbicacionDTO>? response = await _apiService.PostAsync<ResponseApi<StockUbicacionDTO>>("api/Stock", stock);

            if (response != null && response.IsSuccess) {

                return "Stock creado correctamente";

            }

            if (response != null && response.ErrorMessages.Count > 0){

                return response.ErrorMessages[0];

            }

            return "No se pudo crear el stock";
        }
    }



}