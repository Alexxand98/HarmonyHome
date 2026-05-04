using HarmonyHome.Api.Models.DTOs;

namespace HarmonyHome.Api.Repository.IRepository
{
    public interface IOrdenRecogidaRepository
    {
        Task<List<OrdenRecogidaDTO>> GetAll();

        Task<List<OrdenRecogidaDTO>> GetPendientes();

        Task<OrdenRecogidaDTO?> GetById(int id);

        Task<OrdenRecogidaDTO?> Asignar(int id, string usuarioId);

        Task<OrdenRecogidaDTO?> MarcarEnPreparacion(int id);

        Task<OrdenRecogidaDTO?> Finalizar(int id, string usuarioId, FinalizarOrdenRecogidaDTO dto);
    }
}