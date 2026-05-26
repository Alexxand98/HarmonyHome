using HarmonyHome.Api.Models.DTOs;
using HarmonyHome.Api.Models.DTOs.OrdenDto;
using HarmonyHome.Api.Models.DTOs.PreparacionReposicionDto;

namespace HarmonyHome.Api.Repository.IRepository
{
    public interface IOrdenReposicionRepository
    {
        Task<List<OrdenReposicionDTO>> GetAll();

        Task<List<OrdenReposicionDTO>> GetPendientes();

        Task<OrdenReposicionDTO?> GetById(int id);

        Task<OrdenReposicionDTO?> Create(CreateOrdenReposicionDTO dto, string usuarioSolicitanteId);

        Task<OrdenReposicionDTO?> Asignar(int id, string usuarioPreparadorId);

        Task<OrdenReposicionDTO?> Finalizar(int id, string usuarioPreparadorId, FinalizarOrdenReposicionDTO dto);

        Task<PreparacionReposicionDTO?> GetPreparacion(int id);

        Task<OrdenReposicionDTO?> Cancelar(int id, string usuarioId, CancelarOrdenDTO dto);
    }
}