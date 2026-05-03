using System.Net;

namespace HarmonyHome.Api.Models.DTOs
{
    public class ResponseApi
    {
        public HttpStatusCode StatusCode { get; set; }

        public bool IsSuccess { get; set; } = true;

        public object? Result { get; set; }

        public List<string> ErrorMessages { get; set; } = new List<string>();
    }
}