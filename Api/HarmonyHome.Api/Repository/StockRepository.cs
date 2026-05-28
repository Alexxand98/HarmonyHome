using HarmonyHome.Api.Data;
using HarmonyHome.Api.Helpers;
using HarmonyHome.Api.Models.DTOs;
using HarmonyHome.Api.Models.DTOs.MovimientoStockDto;
using HarmonyHome.Api.Models.DTOs.StockDto;
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

            if (!productoExiste || !ubicacionExiste)   {

                return null;
            }

            var stockExistente = await _context.StockUbicaciones.FirstOrDefaultAsync(s =>s.ProductoId == createStockDTO.ProductoId && s.UbicacionId == createStockDTO.UbicacionId);

            if (stockExistente != null) {


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

            if (stock == null) {

                return null;
            }

            stock.Cantidad = updateStockDTO.Cantidad;

            _context.StockUbicaciones.Update(stock);

            await _context.SaveChangesAsync();

            return await GetById(stock.Id);
        }



        public async Task<MovimientoStockDTO?> MoverStock(MoveStockDTO moveStockDTO, string usuarioId)
        {
            if (moveStockDTO.UbicacionOrigenId == moveStockDTO.UbicacionDestinoId)  {

                return null;
            }

            var producto = await _context.Productos.FirstOrDefaultAsync(p => p.Id == moveStockDTO.ProductoId && p.Activo);

            if (producto == null)  {

                return null;
            }

            var ubicacionOrigen = await _context.Ubicaciones.FirstOrDefaultAsync(u => u.Id == moveStockDTO.UbicacionOrigenId && u.Activa);

            var ubicacionDestino = await _context.Ubicaciones.FirstOrDefaultAsync(u => u.Id == moveStockDTO.UbicacionDestinoId && u.Activa);

            if (ubicacionOrigen == null || ubicacionDestino == null)  {
                return null;
            }

            // Movimiento interno: no permitimos que participe TIENDA.
            // La tienda se modifica mediante venta directa, reposición o demarca.

            if (ubicacionOrigen.TipoUbicacion == TipoUbicacion.Tienda || ubicacionDestino.TipoUbicacion == TipoUbicacion.Tienda) {
                
                return null;
            }

            var stockOrigen = await _context.StockUbicaciones.FirstOrDefaultAsync(s => s.ProductoId == moveStockDTO.ProductoId && s.UbicacionId == moveStockDTO.UbicacionOrigenId);

            if (stockOrigen == null || stockOrigen.Cantidad < moveStockDTO.Cantidad) {

                return null;
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                stockOrigen.Cantidad -= moveStockDTO.Cantidad;

                var stockDestino = await _context.StockUbicaciones.FirstOrDefaultAsync(s =>s.ProductoId == moveStockDTO.ProductoId && s.UbicacionId == moveStockDTO.UbicacionDestinoId);

                if (stockDestino == null){

                    stockDestino = new StockUbicacion
                    {
                        ProductoId = moveStockDTO.ProductoId,
                        UbicacionId = moveStockDTO.UbicacionDestinoId,
                        Cantidad = 0
                    };

                    await _context.StockUbicaciones.AddAsync(stockDestino);
                }

                stockDestino.Cantidad += moveStockDTO.Cantidad;

                var movimiento = new MovimientoStock
                {
                    ProductoId = moveStockDTO.ProductoId,
                    UbicacionOrigenId = moveStockDTO.UbicacionOrigenId,
                    UbicacionDestinoId = moveStockDTO.UbicacionDestinoId,
                    Cantidad = moveStockDTO.Cantidad,
                    Fecha = DateTime.UtcNow,
                    UsuarioId = usuarioId,
                    TipoMovimiento = TipoMovimiento.MovimientoManual,
                    Observaciones = moveStockDTO.Observaciones ?? "Movimiento interno manual de stock."
                };

                await _context.MovimientosStock.AddAsync(movimiento);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                var movimientoGuardado = await _context.MovimientosStock
                    .Include(m => m.Producto)
                    .Include(m => m.UbicacionOrigen)
                    .Include(m => m.UbicacionDestino).FirstOrDefaultAsync(m => m.Id == movimiento.Id);

                return movimientoGuardado == null
                    ? null
                    : ApplicationMapper.ToMovimientoStockDTO(movimientoGuardado);
            }catch {
                await transaction.RollbackAsync();
                return null;
            }
        }


        public async Task<List<ProductoBajoStockDTO>> GetProductosBajoStockTienda()
        {
            var productos = await _context.Productos
                .Where(p => p.Activo && p.Habilitado)
                .ToListAsync();

            var resultado = new List<ProductoBajoStockDTO>();

            foreach (var producto in productos)
            {
                var stockTienda = await _context.StockUbicaciones
                    .Include(s => s.Ubicacion)
                    .Where(s =>
                        s.ProductoId == producto.Id &&
                        s.Ubicacion != null &&
                        s.Ubicacion.TipoUbicacion == TipoUbicacion.Tienda)
                    .SumAsync(s => s.Cantidad);

                var stockAlmacen = await _context.StockUbicaciones
                    .Include(s => s.Ubicacion)
                    .Where(s =>
                        s.ProductoId == producto.Id &&
                        s.Ubicacion != null &&
                        s.Ubicacion.TipoUbicacion == TipoUbicacion.Almacen)
                    .SumAsync(s => s.Cantidad);

                var stockTotal = stockTienda + stockAlmacen;

                if (stockTienda <= producto.StockMinimo)
                {
                    resultado.Add(new ProductoBajoStockDTO
                    {
                        ProductoId = producto.Id,
                        Referencia = producto.Referencia,
                        Nombre = producto.Nombre,
                        Categoria = producto.Categoria,
                        ImagenUrl = producto.ImagenUrl ?? string.Empty,
                        StockMinimo = producto.StockMinimo,
                        StockTienda = stockTienda,
                        StockAlmacen = stockAlmacen,
                        StockTotal = stockTotal,
                        StockEvaluado = stockTienda,
                        TipoEvaluacion = "Tienda"
                    });
                }
            }

            return resultado;
        }

        public async Task<List<ProductoBajoStockDTO>> GetProductosBajoStockGeneral()
        {
            var productos = await _context.Productos
                .Where(p => p.Activo)
                .ToListAsync();

            var resultado = new List<ProductoBajoStockDTO>();

            foreach (var producto in productos)
            {
                var stockTienda = await _context.StockUbicaciones
                    .Include(s => s.Ubicacion)
                    .Where(s =>
                        s.ProductoId == producto.Id &&
                        s.Ubicacion != null &&
                        s.Ubicacion.TipoUbicacion == TipoUbicacion.Tienda)
                    .SumAsync(s => s.Cantidad);

                var stockAlmacen = await _context.StockUbicaciones
                    .Include(s => s.Ubicacion)
                    .Where(s =>
                        s.ProductoId == producto.Id &&
                        s.Ubicacion != null &&
                        s.Ubicacion.TipoUbicacion == TipoUbicacion.Almacen)
                    .SumAsync(s => s.Cantidad);

                var stockTotal = await _context.StockUbicaciones
                    .Include(s => s.Ubicacion)
                    .Where(s =>
                        s.ProductoId == producto.Id &&
                        s.Ubicacion != null &&
                        s.Ubicacion.Activa)
                    .SumAsync(s => s.Cantidad);

                if (stockTotal <= producto.StockMinimo)
                {
                    resultado.Add(new ProductoBajoStockDTO
                    {
                        ProductoId = producto.Id,
                        Referencia = producto.Referencia,
                        Nombre = producto.Nombre,
                        Categoria = producto.Categoria,
                        ImagenUrl = producto.ImagenUrl ?? string.Empty,
                        StockMinimo = producto.StockMinimo,
                        StockTienda = stockTienda,
                        StockAlmacen = stockAlmacen,
                        StockTotal = stockTotal,
                        StockEvaluado = stockTotal,
                        TipoEvaluacion = "General"
                    });
                }
            }

            return resultado;
        }
    }
}