namespace HarmonyHome.Api.Models.DTOs
{
    public class UpdateUbicacionDTO : CreateUbicacionDTO
    {
        public bool Activa { get; set; } = true;
    }
}