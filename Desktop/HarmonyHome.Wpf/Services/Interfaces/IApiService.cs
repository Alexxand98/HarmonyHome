using System;
using System.Collections.Generic;
using System.Text;

namespace HarmonyHome.Wpf.Services.Interfaces
{
    public interface IApiService
    {
        Task<T?> GetAsync<T>(string endpoint);

        Task<T?> PostAsync<T>(string endpoint, object data);

        Task<T?> PutAsync<T>(string endpoint, object data);

        Task<T?> DeleteAsync<T>(string endpoint);
    

    }
}