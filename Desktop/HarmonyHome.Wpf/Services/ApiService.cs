using HarmonyHome.Wpf.Helpers;
using HarmonyHome.Wpf.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace HarmonyHome.Wpf.Services
{
    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService()
        {
            _httpClient = new HttpClient();

            _httpClient.BaseAddress = new Uri(ApiSettings.BaseUrl);
        }

      

        public async Task<T?> GetAsync<T>(string endpoint)
        {
            try{
               // MessageBox.Show("Entrando a ApiService GET: " + endpoint);
                AddToken();

                HttpResponseMessage response = await _httpClient.GetAsync(endpoint);

                string json = await response.Content.ReadAsStringAsync();


                if (!response.IsSuccessStatusCode){
                    return default;
                }


                return JsonSerializer.Deserialize<T>(json, GetJsonOptions());

            }catch{
                MessageBox.Show("Error al leer datos de la API");

                return default;
            }
        }


        public async Task<T?> PostAsync<T>(string endpoint, object data)
        {
            try{
                AddToken();

                string jsonData = JsonSerializer.Serialize(data);

                StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.PostAsync(endpoint, content);

                string json = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode){
                    return default;
                }

                return JsonSerializer.Deserialize<T>(json, GetJsonOptions());

            }catch{
                return default;
            }
        }
        public async Task<T?> PutAsync<T>(string endpoint, object data)
        {
            try{
                AddToken();

                string jsonData = JsonSerializer.Serialize(data);

                StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.PutAsync(endpoint, content);

                string json = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode){
                    return default;
                }

                return JsonSerializer.Deserialize<T>(json, GetJsonOptions());

            }catch{

                return default;

            }
        }
        public async Task<T?> DeleteAsync<T>(string endpoint)
        {
            try{
                AddToken();

                HttpResponseMessage response = await _httpClient.DeleteAsync(endpoint);

                string json = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode){
                    return default;
                }

                return JsonSerializer.Deserialize<T>(json, GetJsonOptions());
            } catch {
                return default;
            }
        }



        private void AddToken()
        {
            if (!string.IsNullOrWhiteSpace(SessionManager.Token)){
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", SessionManager.Token);

            } else{

                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
        }


        private JsonSerializerOptions GetJsonOptions()
        {
            return new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }


        public async Task<T?> PatchAsync<T>(string endpoint, object? data = null)
        {
            try{
                AddToken();

                HttpContent? content = null;

                if (data != null) {
                    string jsonData = JsonSerializer.Serialize(data);

                    content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                }

                HttpResponseMessage response = await _httpClient.PatchAsync(endpoint, content);

                string json = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode) {
                    return default;
                }

                return JsonSerializer.Deserialize<T>(json, GetJsonOptions());

            }catch {
                return default;
            }
        }
    }
}