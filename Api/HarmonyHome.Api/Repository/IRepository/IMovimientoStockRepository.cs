using HarmonyHome.Api.Models.DTOs.MovimientoStockDto;

namespace HarmonyHome.Api.Repository.IRepository
{
    public interface IMovimientoStockRepository
    {
        Task<List<MovimientoStockDTO>> GetAll();

        Task<List<MovimientoStockDTO>> GetByProducto(int productoId);

        Task<List<MovimientoStockDTO>> GetByUbicacion(int ubicacionId);

        Task<MovimientoStockDTO?> GetById(int id);
    }
}