using HarmonyHome.Wpf.Models.DTOs;
using HarmonyHome.Wpf.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace HarmonyHome.Wpf.Services
{
    public class ProductoService
    {
        private readonly IApiService _apiService;

        public ProductoService()
        {
            _apiService = new ApiService();
        }

        public async Task<List<ProductoDTO>> GetProductosAsync()
        {
            ResponseApi<List<ProductoDTO>>? response = await _apiService.GetAsync<ResponseApi<List<ProductoDTO>>>("api/Producto");

            if (response != null && response.IsSuccess && response.Result != null)
            {
                return response.Result;
            }

            return new List<ProductoDTO>();
        }

        public async Task<ProductoDTO?> GetProductoByIdAsync(int id)
        {
            ResponseApi<ProductoDTO>? response = await _apiService.GetAsync<ResponseApi<ProductoDTO>>($"api/Producto/{id}");

            if (response != null && response.IsSuccess)
            {
                return response.Result;
            }

            return null;
        }

        public async Task<bool> CrearProductoAsync(CreateProductoDTO producto)
        {
            ResponseApi<ProductoDTO>? response = await _apiService.PostAsync<ResponseApi<ProductoDTO>>("api/Producto", producto);

            return response != null && response.IsSuccess;
        }

        public async Task<bool> ActualizarProductoAsync(int id, UpdateProductoDTO producto)
        {
            ResponseApi<ProductoDTO>? response = await _apiService.PutAsync<ResponseApi<ProductoDTO>>($"api/Producto/{id}", producto);

            return response != null && response.IsSuccess;
        }

        public async Task<string> EliminarProductoAsync(int id)
        {
            ResponseApi<string>? response = await _apiService.DeleteAsync<ResponseApi<string>>($"api/Producto/{id}");

            if (response != null && response.IsSuccess && response.Result != null) {
                return response.Result;
            }

            if (response != null && response.ErrorMessages.Count > 0)  {
                return response.ErrorMessages[0];
            }

            return "No se pudo eliminar el producto.";
        }

        public async Task<bool> HabilitarProductoAsync(int id)
        {
            ResponseApi<bool>? response = await _apiService.PatchAsync<ResponseApi<bool>>($"api/Producto/{id}/habilitar");

            return response != null && response.IsSuccess;
        }

        public async Task<bool> DeshabilitarProductoAsync(int id)
        {
            ResponseApi<bool>? response = await _apiService.PatchAsync<ResponseApi<bool>>($"api/Producto/{id}/deshabilitar");

            return response != null && response.IsSuccess;
        }

        public async Task<List<ProductoDTO>> BuscarProductosAsync(string texto)
        {
            string textoSeguro = Uri.EscapeDataString(texto);

            ResponseApi<List<ProductoDTO>>? response = await _apiService.GetAsync<ResponseApi<List<ProductoDTO>>>($"api/Producto/buscar?texto={textoSeguro}");

            if (response != null && response.IsSuccess && response.Result != null) {

                return response.Result;
            }

            return new List<ProductoDTO>();
        }

    }
}