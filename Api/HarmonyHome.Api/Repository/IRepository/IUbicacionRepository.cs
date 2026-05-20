using HarmonyHome.Api.Models.DTO;
using HarmonyHome.Api.Models.DTOs;
using HarmonyHome.Api.Models.Enums;

namespace HarmonyHome.Api.Repository.IRepository
{
    public interface IUbicacionRepository
    {
        Task<List<UbicacionDTO>> GetAll();

        Task<List<UbicacionDTO>> GetActivas();

        Task<List<UbicacionDTO>> GetByTipo(TipoUbicacion tipoUbicacion);

        Task<UbicacionDTO?> GetById(int id);

        Task<UbicacionDTO?> GetByCodigo(string codigo);

        Task<UbicacionDTO?> Create(CreateUbicacionDTO createUbicacionDTO);

        Task<UbicacionDTO?> Update(int id, UpdateUbicacionDTO updateUbicacionDTO);

        Task<string?> Delete(int id);
        Task<bool> ExisteCodigo(string codigo);

        Task<bool> ExisteCodigoEnOtraUbicacion(string codigo, int id);
    }
}