namespace HarmonyHome.Api.Models.DTOs
{
    public class UpdateClienteDTO : CreateClienteDTO
    {
        public bool Activo { get; set; } = true;
    }
}