namespace HarmonyHome.Api.Models.DTOs
{
    public class UpdateProductoDTO : CreateProductoDTO
    {
        public bool Activo { get; set; } = true;
    }
}