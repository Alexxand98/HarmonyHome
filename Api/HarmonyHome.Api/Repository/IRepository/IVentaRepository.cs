using HarmonyHome.Api.Models.DTOs;
using HarmonyHome.Api.Models.DTOs.VentaMixtaDto;

namespace HarmonyHome.Api.Repository.IRepository
{
    public interface IVentaRepository
    {
        Task<VentaDTO?> CrearVentaDirecta(CreateVentaDirectaDTO createVentaDirectaDTO, string usuarioId);

        Task<VentaMixtaDTO?> CrearVentaMixta(CreateVentaMixtaDTO dto, string usuarioId);
    }
}