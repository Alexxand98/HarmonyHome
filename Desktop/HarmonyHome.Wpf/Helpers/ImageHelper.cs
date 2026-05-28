using System;
using System.Collections.Generic;
using System.Text;

namespace HarmonyHome.Wpf.Helpers
{
    public static class ImageHelper
    {
        public static string GetProductoImageUrl(string? imagenUrl)
        {
            if (string.IsNullOrWhiteSpace(imagenUrl))
            {
                return "";
            }

            if (imagenUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                return imagenUrl;
            }

            string baseUrl = ApiSettings.BaseUrl.TrimEnd('/');
            string rutaImagen = imagenUrl.TrimStart('/');

            return baseUrl + "/" + rutaImagen;
        }
    }
}