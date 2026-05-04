using HarmonyHome.Api.Models.DTOs;

namespace HarmonyHome.Api.Repository.IRepository
{
    public interface IPedidoClienteRepository
    {
        Task<PedidoClienteDTO?> CrearPedidoCliente(CreatePedidoClienteDTO createPedidoClienteDTO, string usuarioId);
    }
}