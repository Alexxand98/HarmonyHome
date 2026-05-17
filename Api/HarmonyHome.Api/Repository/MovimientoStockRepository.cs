using HarmonyHome.Api.Data;
using HarmonyHome.Api.Helpers;
using HarmonyHome.Api.Models.DTOs.MovimientoStockDto;
using HarmonyHome.Api.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace HarmonyHome.Api.Repository
{
    public class MovimientoStockRepository : IMovimientoStockRepository
    {
        private readonly ApplicationDbContext _context;

        public MovimientoStockRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<MovimientoStockDTO>> GetAll()
        {
            return await _context.MovimientosStock
                .Include(m => m.Producto)
                .Include(m => m.UbicacionOrigen)
                .Include(m => m.UbicacionDestino)
                .OrderByDescending(m => m.Fecha)
                .Select(m => ApplicationMapper.ToMovimientoStockDTO(m))
                .ToListAsync();
        }

        public async Task<List<MovimientoStockDTO>> GetByProducto(int productoId)
        {
            return await _context.MovimientosStock
                .Include(m => m.Producto)
                .Include(m => m.UbicacionOrigen)
                .Include(m => m.UbicacionDestino)
                .Where(m => m.ProductoId == productoId)
                .OrderByDescending(m => m.Fecha)
                .Select(m => ApplicationMapper.ToMovimientoStockDTO(m))
                .ToListAsync();
        }

        public async Task<List<MovimientoStockDTO>> GetByUbicacion(int ubicacionId)
        {
            return await _context.MovimientosStock
                .Include(m => m.Producto)
                .Include(m => m.UbicacionOrigen)
                .Include(m => m.UbicacionDestino)
                .Where(m =>
                    m.UbicacionOrigenId == ubicacionId ||
                    m.UbicacionDestinoId == ubicacionId)
                .OrderByDescending(m => m.Fecha)
                .Select(m => ApplicationMapper.ToMovimientoStockDTO(m))
                .ToListAsync();
        }

        public async Task<MovimientoStockDTO?> GetById(int id)
        {
            var movimiento = await _context.MovimientosStock
                .Include(m => m.Producto)
                .Include(m => m.UbicacionOrigen)
                .Include(m => m.UbicacionDestino)
                .FirstOrDefaultAsync(m => m.Id == id);

            return movimiento == null
                ? null
                : ApplicationMapper.ToMovimientoStockDTO(movimiento);
        }
    }
}