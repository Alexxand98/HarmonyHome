using HarmonyHome.Wpf.Models.DTOs;
using HarmonyHome.Wpf.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace HarmonyHome.Wpf.Services
{
    public class ProductoService : IProductoService
    {
        private readonly IApiService _apiService;

        public ProductoService()
        {
            _apiService = new ApiService();

        }

        public async Task<List<ProductoDTO>> GetProductosAsync(){

            MessageBox.Show("Entrando a ProductoService");

            ResponseApi<List<ProductoDTO>>? response = await _apiService.GetAsync<ResponseApi<List<ProductoDTO>>>("api/Producto");

            if (response == null) {
                return new List<ProductoDTO>();
            }


            if (!response.IsSuccess || response.Result == null){
                return new List<ProductoDTO>();

            }


            return response.Result;
        }

    }

}