using HarmonyHome.Api.Models.DTOs;
using HarmonyHome.Api.Models.DTOs.MovimientoStockDto;
using HarmonyHome.Api.Models.DTOs.StockDto;

namespace HarmonyHome.Api.Repository.IRepository
{
    public interface IStockRepository
    {
        Task<List<StockUbicacionDTO>> GetAll();

        Task<List<StockUbicacionDTO>> GetByProducto(int productoId);

        Task<List<StockUbicacionDTO>> GetByUbicacion(int ubicacionId);

        Task<List<StockUbicacionDTO>> GetStockTienda();

        Task<List<StockUbicacionDTO>> GetStockAlmacen();

        Task<StockResumenDTO?> GetResumenByProducto(int productoId);

        Task<StockUbicacionDTO?> GetById(int id);

        Task<StockUbicacionDTO?> Create(CreateStockUbicacionDTO createStockDTO);

        Task<StockUbicacionDTO?> Update(int id, UpdateStockUbicacionDTO updateStockDTO);

        Task<MovimientoStockDTO?> MoverStock(MoveStockDTO moveStockDTO, string usuarioId);

    }
}