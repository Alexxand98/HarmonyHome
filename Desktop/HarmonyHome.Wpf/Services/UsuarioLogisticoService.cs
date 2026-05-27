using HarmonyHome.Wpf.Models.DTOs;
using HarmonyHome.Wpf.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace HarmonyHome.Wpf.Services
{
    public class UsuarioLogisticoService
    {
        private readonly IApiService _apiService;

        public UsuarioLogisticoService()
        {
            _apiService = new ApiService();
        }

        public async Task<List<UsuarioLogisticoDTO>> GetLogisticosAsync()
        {
            ResponseApi<List<UsuarioLogisticoDTO>>? response = await _apiService.GetAsync<ResponseApi<List<UsuarioLogisticoDTO>>>("api/User/logisticos");

            if (response != null && response.IsSuccess && response.Result != null){
                return response.Result;
            }

            return new List<UsuarioLogisticoDTO>();
        }

        public async Task<UsuarioLogisticoDTO?> GetUsuarioByIdAsync(string id)
        {
            ResponseApi<UsuarioLogisticoDTO>? response = await _apiService.GetAsync<ResponseApi<UsuarioLogisticoDTO>>($"api/User/{id}");

            if (response != null && response.IsSuccess){

                return response.Result;
            }

            return null;
        }

        public async Task<bool> CrearLogisticoAsync(CreateUsuarioLogisticoDTO usuario)
        {
            ResponseApi<UsuarioLogisticoDTO>? response = await _apiService.PostAsync<ResponseApi<UsuarioLogisticoDTO>>("api/User/logistico", usuario);


            return response != null && response.IsSuccess;
        }

        public async Task<bool> ActualizarLogisticoAsync(string id, UpdateUsuarioLogisticoDTO usuario)
        {
            ResponseApi<UsuarioLogisticoDTO>? response = await _apiService.PutAsync<ResponseApi<UsuarioLogisticoDTO>>($"api/User/{id}", usuario);

            return response != null && response.IsSuccess;
        }

        public async Task<bool> ActivarLogisticoAsync(string id)
        {
            ResponseApi<string>? response = await _apiService.PatchAsync<ResponseApi<string>>($"api/User/{id}/activar");


            return response != null && response.IsSuccess;
        }

        public async Task<bool> DesactivarLogisticoAsync(string id)
        {
            ResponseApi<string>? response = await _apiService.PatchAsync<ResponseApi<string>>($"api/User/{id}/desactivar");


            return response != null && response.IsSuccess;

        }

        public async Task<string> EliminarLogisticoAsync(string id)
        {
            ResponseApi<string>? response = await _apiService.DeleteAsync<ResponseApi<string>>($"api/User/{id}");

            if (response != null && response.IsSuccess && response.Result != null){
                return response.Result;
            }

            if (response != null && response.ErrorMessages.Count > 0){
                return response.ErrorMessages[0];
            }

            return "No se pudo eliminar el logístico";
        }
    }
}