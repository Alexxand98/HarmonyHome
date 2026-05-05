using HarmonyHome.Api.Models.DTOs;

namespace HarmonyHome.Api.Repository.IRepository
{
    public interface IDemarcaRepository
    {
        Task<DemarcaDTO?> CrearDemarca(CreateDemarcaDTO dto, string usuarioId, string rolUsuario);
    }
}