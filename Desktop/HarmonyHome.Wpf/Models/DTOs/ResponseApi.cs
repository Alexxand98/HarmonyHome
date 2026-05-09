using System;
using System.Collections.Generic;
using System.Text;

namespace HarmonyHome.Wpf.Models.DTOs
{
    public class ResponseApi<T>
    {
        public int StatusCode { get; set; }

        public bool IsSuccess { get; set; }

        public T? Result { get; set; }

        public List<string> ErrorMessages { get; set; } = new List<string>();
    }
}