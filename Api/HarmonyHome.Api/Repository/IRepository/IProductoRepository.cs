using HarmonyHome.Api.Models.DTOs;

namespace HarmonyHome.Api.Repository.IRepository
{
    public interface IProductoRepository
    {
        Task<List<ProductoDTO>> GetAll();

        Task<List<ProductoDTO>> GetHabilitados();

        Task<ProductoDTO?> GetById(int id);

        Task<ProductoDTO?> GetByReferencia(string referencia);

        Task<List<ProductoDTO>> Buscar(string? texto, string? categoria);

        Task<ProductoDTO?> Create(CreateProductoDTO createProductoDTO);

        Task<ProductoDTO?> Update(int id, UpdateProductoDTO updateProductoDTO);

        Task<bool> Delete(int id);

        Task<bool> Habilitar(int id);

        Task<bool> Deshabilitar(int id);

        Task<bool> ExisteReferencia(string referencia);

        Task<bool> ExisteReferenciaEnOtroProducto(string referencia, int id);
    }
}