using HarmonyHome.Api.Data;
using HarmonyHome.Api.Helpers;
using HarmonyHome.Api.Models.DTOs;
using HarmonyHome.Api.Models.Entity;
using HarmonyHome.Api.Models.Enums;
using HarmonyHome.Api.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace HarmonyHome.Api.Repository
{
    public class OrdenReposicionRepository : IOrdenReposicionRepository
    {
        private readonly ApplicationDbContext _context;

        public OrdenReposicionRepository(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<List<OrdenReposicionDTO>> GetAll()
        {
            return await _context.OrdenesReposicion
                .Include(o => o.Lineas)
                    .ThenInclude(l => l.Producto)
                .OrderByDescending(o => o.FechaSolicitud)
                .Select(o => ApplicationMapper.ToOrdenReposicionDTO(o))
                .ToListAsync();
        }
        public async Task<List<OrdenReposicionDTO>> GetPendientes()
        {
            return await _context.OrdenesReposicion
                .Include(o => o.Lineas)
                    .ThenInclude(l => l.Producto)
                .Where(o => o.Estado == EstadoOrden.Pendiente)
                .OrderBy(o => o.FechaSolicitud)
                .Select(o => ApplicationMapper.ToOrdenReposicionDTO(o))
                .ToListAsync();
        }


        public async Task<OrdenReposicionDTO?> GetById(int id)
        {
            var orden = await _context.OrdenesReposicion.Include(o => o.Lineas).ThenInclude(l => l.Producto).FirstOrDefaultAsync(o => o.Id == id);

            return orden == null ? null : ApplicationMapper.ToOrdenReposicionDTO(orden);
        }



        public async Task<OrdenReposicionDTO?> Create(CreateOrdenReposicionDTO dto, string usuarioSolicitanteId)
        {
            foreach (var linea in dto.Lineas)
            {
                var productoExiste = await _context.Productos
                    .AnyAsync(p => p.Id == linea.ProductoId && p.Activo && p.Habilitado);

                if (!productoExiste)
                {
                    return null;
                }

                var stockAlmacen = await _context.StockUbicaciones
                    .Include(s => s.Ubicacion)
                    .Where(s =>
                        s.ProductoId == linea.ProductoId &&
                        s.Ubicacion != null &&
                        s.Ubicacion.TipoUbicacion == TipoUbicacion.Almacen)
                    .SumAsync(s => s.Cantidad);

                if (stockAlmacen < linea.CantidadSolicitada)
                {
                    return null;
                }
            }

            var orden = new OrdenReposicion
            {
                FechaSolicitud = DateTime.UtcNow,
                Estado = EstadoOrden.Pendiente,
                UsuarioSolicitanteId = usuarioSolicitanteId,
                Observaciones = dto.Observaciones
            };

            foreach (var linea in dto.Lineas)
            {
                orden.Lineas.Add(new LineaOrdenReposicion
                {
                    ProductoId = linea.ProductoId,
                    CantidadSolicitada = linea.CantidadSolicitada,
                    CantidadPreparada = 0
                });
            }

            await _context.OrdenesReposicion.AddAsync(orden);
            await _context.SaveChangesAsync();

            return await GetById(orden.Id);
        }

       
        
        public async Task<OrdenReposicionDTO?> Asignar(int id, string usuarioPreparadorId)
        {
            var orden = await _context.OrdenesReposicion
                .FirstOrDefaultAsync(o => o.Id == id);

            if (orden == null || orden.Estado != EstadoOrden.Pendiente)
            {
                return null;
            }

            orden.UsuarioPreparadorId = usuarioPreparadorId;
            orden.Estado = EstadoOrden.Asignada;

            await _context.SaveChangesAsync();

            return await GetById(id);
        }

       
        
        public async Task<OrdenReposicionDTO?> Finalizar(int id, string usuarioPreparadorId, FinalizarOrdenReposicionDTO dto)
        
        {
            var orden = await _context.OrdenesReposicion
                .Include(o => o.Lineas)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (orden == null)
            {
               
                return null;
            }

           
            if (orden.Estado != EstadoOrden.Asignada && orden.Estado != EstadoOrden.EnPreparacion)
            {
               
                
                return null;
            }

            var ubicacionTienda = await _context.Ubicaciones
                .FirstOrDefaultAsync(u => u.TipoUbicacion == TipoUbicacion.Tienda && u.Activa);

           
            if (ubicacionTienda == null)
            {
                return null;
            }

           
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                foreach (var linea in orden.Lineas)
                {
                    var cantidadPendiente = linea.CantidadSolicitada;

                    var stocksAlmacen = await _context.StockUbicaciones
                        .Include(s => s.Ubicacion)
                        .Where(s =>
                            s.ProductoId == linea.ProductoId &&
                            s.Ubicacion != null &&
                            s.Ubicacion.TipoUbicacion == TipoUbicacion.Almacen &&
                            s.Cantidad > 0)
                        .OrderBy(s => s.Ubicacion!.Codigo)
                        .ToListAsync();

                            
                    var totalDisponible = stocksAlmacen.Sum(s => s.Cantidad);             
                    
                    if (totalDisponible < cantidadPendiente)
                    {
                        await transaction.RollbackAsync();
                        return null;
                    }

                    foreach (var stockOrigen in stocksAlmacen)
                    {
                        if (cantidadPendiente <= 0)
                        {
                            break;
                        }

                        var cantidadMover = Math.Min(stockOrigen.Cantidad, cantidadPendiente);

                        stockOrigen.Cantidad -= cantidadMover;

                        cantidadPendiente -= cantidadMover;

                        var stockTienda = await _context.StockUbicaciones.FirstOrDefaultAsync(s =>s.ProductoId == linea.ProductoId && s.UbicacionId== ubicacionTienda.Id);

                        if (stockTienda == null)
                        {
                            stockTienda = new StockUbicacion
                            {
                                ProductoId = linea.ProductoId,
                                UbicacionId = ubicacionTienda.Id,
                                Cantidad = 0
                            };


                            await _context.StockUbicaciones.AddAsync(stockTienda);
                        }

                        stockTienda.Cantidad += cantidadMover;

                        await _context.MovimientosStock.AddAsync(new MovimientoStock
                        {
                            ProductoId = linea.ProductoId,
                            UbicacionOrigenId = stockOrigen.UbicacionId,
                            UbicacionDestinoId = ubicacionTienda.Id,
                            Cantidad = cantidadMover,
                            Fecha = DateTime.UtcNow,
                            UsuarioId = usuarioPreparadorId,
                            TipoMovimiento = TipoMovimiento.Reposicion,
                            Observaciones = $"Reposición finalizada. OrdenReposicionId: {orden.Id}"
                        });
                    }

                    linea.CantidadPreparada = linea.CantidadSolicitada;
                }

                orden.UsuarioPreparadorId ??= usuarioPreparadorId;

                orden.Estado = EstadoOrden.Finalizada;

                orden.Observaciones = dto.Observaciones ?? orden.Observaciones;

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return await GetById(id);
            }
            catch
            {
                await transaction.RollbackAsync();

                return null;
            }
        }
    }
}