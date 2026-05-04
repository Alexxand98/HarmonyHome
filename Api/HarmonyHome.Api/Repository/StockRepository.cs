using HarmonyHome.Api.Data;
using HarmonyHome.Api.Helpers;
using HarmonyHome.Api.Models.DTOs;
using HarmonyHome.Api.Models.Entity;
using HarmonyHome.Api.Models.Enums;
using HarmonyHome.Api.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace HarmonyHome.Api.Repository
{
    public class StockRepository : IStockRepository
    {
        private readonly ApplicationDbContext _context;

        public StockRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<StockUbicacionDTO>> GetAll()
        {
            return await _context.StockUbicaciones
                .Include(s => s.Producto)
                .Include(s => s.Ubicacion)
                .OrderBy(s => s.Producto!.Nombre)
                .ThenBy(s => s.Ubicacion!.Codigo)
                .Select(s => ApplicationMapper.ToStockUbicacionDTO(s))
                .ToListAsync();
        }


        public async Task<List<StockUbicacionDTO>> GetByProducto(int productoId)
        {
            return await _context.StockUbicaciones
                .Include(s => s.Producto)
                .Include(s => s.Ubicacion)
                .Where(s => s.ProductoId == productoId)
                .OrderBy(s => s.Ubicacion!.Codigo)
                .Select(s => ApplicationMapper.ToStockUbicacionDTO(s))
                .ToListAsync();
        }

        public async Task<List<StockUbicacionDTO>> GetByUbicacion(int ubicacionId)
        {
            return await _context.StockUbicaciones
                .Include(s => s.Producto)
                .Include(s => s.Ubicacion)
                .Where(s => s.UbicacionId == ubicacionId)
                .OrderBy(s => s.Producto!.Nombre)
                .Select(s => ApplicationMapper.ToStockUbicacionDTO(s))
                .ToListAsync();
        }


        public async Task<List<StockUbicacionDTO>> GetStockTienda()
        {
            return await _context.StockUbicaciones
                .Include(s => s.Producto)
                .Include(s => s.Ubicacion)
                .Where(s => s.Ubicacion!.TipoUbicacion == TipoUbicacion.Tienda)
                .OrderBy(s => s.Producto!.Nombre)
                .Select(s => ApplicationMapper.ToStockUbicacionDTO(s))
                .ToListAsync();
        }

        public async Task<List<StockUbicacionDTO>> GetStockAlmacen()
        {
            return await _context.StockUbicaciones
                .Include(s => s.Producto)
                .Include(s => s.Ubicacion)
                .Where(s => s.Ubicacion!.TipoUbicacion == TipoUbicacion.Almacen)
                .OrderBy(s => s.Producto!.Nombre)
                .Select(s => ApplicationMapper.ToStockUbicacionDTO(s))
                .ToListAsync();
        }


        public async Task<StockResumenDTO?> GetResumenByProducto(int productoId)
        {
            var producto = await _context.Productos.FirstOrDefaultAsync(p => p.Id == productoId);

            if (producto == null)
            {
                return null;
            }

            var stocks = await _context.StockUbicaciones
                .Include(s => s.Ubicacion)
                .Where(s => s.ProductoId == productoId).ToListAsync();

            var stockTienda = stocks
                .Where(s => s.Ubicacion != null && s.Ubicacion.TipoUbicacion == TipoUbicacion.Tienda)
                .Sum(s => s.Cantidad);

            var stockAlmacen = stocks
                .Where(s => s.Ubicacion != null && s.Ubicacion.TipoUbicacion == TipoUbicacion.Almacen)
                .Sum(s => s.Cantidad);

            return new StockResumenDTO
            {
                ProductoId = producto.Id,
                ProductoReferencia = producto.Referencia,
                ProductoNombre = producto.Nombre,
                StockTienda = stockTienda,
                StockAlmacen = stockAlmacen,
                StockTotal = stockTienda + stockAlmacen
            };
        }

        public async Task<StockUbicacionDTO?> GetById(int id)
        {
            var stock = await _context.StockUbicaciones
                .Include(s => s.Producto)
                .Include(s => s.Ubicacion)
                .FirstOrDefaultAsync(s => s.Id == id);

            return stock == null ? null : ApplicationMapper.ToStockUbicacionDTO(stock);
        }


        public async Task<StockUbicacionDTO?> Create(CreateStockUbicacionDTO createStockDTO)
        {
            var productoExiste = await _context.Productos.AnyAsync(p => p.Id == createStockDTO.ProductoId && p.Activo);
            var ubicacionExiste = await _context.Ubicaciones.AnyAsync(u => u.Id == createStockDTO.UbicacionId && u.Activa);

            if (!productoExiste || !ubicacionExiste)
            {
                return null;
            }

            var stockExistente = await _context.StockUbicaciones
                .FirstOrDefaultAsync(s =>s.ProductoId == createStockDTO.ProductoId && s.UbicacionId == createStockDTO.UbicacionId);

            if (stockExistente != null)
            {
                stockExistente.Cantidad += createStockDTO.Cantidad;

                _context.StockUbicaciones.Update(stockExistente);

                await _context.SaveChangesAsync();

                return await GetById(stockExistente.Id);
            }

            var stock = new StockUbicacion
            {
                ProductoId = createStockDTO.ProductoId,
                UbicacionId = createStockDTO.UbicacionId,

                Cantidad = createStockDTO.Cantidad
            };

            await _context.StockUbicaciones.AddAsync(stock);

            await _context.SaveChangesAsync();

            return await GetById(stock.Id);
        }


        public async Task<StockUbicacionDTO?> Update(int id, UpdateStockUbicacionDTO updateStockDTO)
        {
            var stock = await _context.StockUbicaciones.FirstOrDefaultAsync(s => s.Id == id);

            if (stock == null)
            {
                return null;
            }

            stock.Cantidad = updateStockDTO.Cantidad;

            _context.StockUbicaciones.Update(stock);

            await _context.SaveChangesAsync();

            return await GetById(stock.Id);
        }
    }
}