using HarmonyHome.Api.Models.DTOs;

namespace HarmonyHome.Api.Repository.IRepository
{
    public interface IVentaRepository
    {
        Task<VentaDTO?> CrearVentaDirecta(CreateVentaDirectaDTO createVentaDirectaDTO, string usuarioId);
    }
}