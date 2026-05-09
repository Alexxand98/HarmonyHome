using HarmonyHome.Api.Models.DTOs;

namespace HarmonyHome.Api.Repository.IRepository
{
    public interface IClienteRepository
    {
        Task<List<ClienteDTO>> GetAll();

        Task<List<ClienteDTO>> GetActivos();

        Task<ClienteDTO?> GetById(int id);

        Task<List<ClienteDTO>> Buscar(string? texto);

        Task<ClienteDTO?> Create(CreateClienteDTO createClienteDTO);

        Task<ClienteDTO?> Update(int id, UpdateClienteDTO updateClienteDTO);

        Task<bool> Delete(int id);
    }
}