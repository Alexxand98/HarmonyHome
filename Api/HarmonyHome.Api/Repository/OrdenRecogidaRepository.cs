using HarmonyHome.Api.Data;
using HarmonyHome.Api.Helpers;
using HarmonyHome.Api.Models.DTOs;
using HarmonyHome.Api.Models.DTOs.OrdenDto;
using HarmonyHome.Api.Models.DTOs.PreparacionRecogidaDto;
using HarmonyHome.Api.Models.Entity;
using HarmonyHome.Api.Models.Enums;
using HarmonyHome.Api.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace HarmonyHome.Api.Repository
{
    public class OrdenRecogidaRepository : IOrdenRecogidaRepository
    {
        private readonly ApplicationDbContext _context;

        public OrdenRecogidaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<OrdenRecogidaDTO>> GetAll()
        {
            var ordenes = await _context.OrdenesRecogida
                .Include(o => o.PedidoVenta)
                    .ThenInclude(p => p!.Cliente)
                .Include(o => o.PedidoVenta)
                    .ThenInclude(p => p!.Lineas)
                        .ThenInclude(l => l.Producto)
                .OrderByDescending(o => o.FechaCreacion)
                .ToListAsync();

            var resultado = new List<OrdenRecogidaDTO>();

            foreach (var orden in ordenes){

                var dto = await MapearOrdenRecogidaCompleta(orden);

                if (dto != null){

                    resultado.Add(dto);
                }
            }

            return resultado;
        }

        public async Task<List<OrdenRecogidaDTO>> GetPendientes()
        {
            var ordenes = await _context.OrdenesRecogida
                .Include(o => o.PedidoVenta)
                    .ThenInclude(p => p!.Cliente)
                .Include(o => o.PedidoVenta)
                    .ThenInclude(p => p!.Lineas)
                        .ThenInclude(l => l.Producto)
                .Where(o => o.Estado == EstadoOrden.Pendiente)
                .OrderBy(o => o.FechaCreacion)
                .ToListAsync();

            var resultado = new List<OrdenRecogidaDTO>();

            foreach (var orden in ordenes){

                var dto = await MapearOrdenRecogidaCompleta(orden);

                if (dto != null){

                    resultado.Add(dto);
                }
            }

            return resultado;
        }

        public async Task<OrdenRecogidaDTO?> GetById(int id)
        {
            var orden = await _context.OrdenesRecogida.Include(o => o.PedidoVenta).ThenInclude(p => p!.Cliente).Include(o => o.PedidoVenta).ThenInclude(p => p!.Lineas).ThenInclude(l => l.Producto).FirstOrDefaultAsync(o => o.Id == id);

            return await MapearOrdenRecogidaCompleta(orden);
        }

        public async Task<OrdenRecogidaDTO?> Asignar(int id, string usuarioId)
        {
            var orden = await _context.OrdenesRecogida
                .FirstOrDefaultAsync(o => o.Id == id);

            if (orden == null || orden.Estado != EstadoOrden.Pendiente){

                return null;
            }

            orden.UsuarioAsignadoId = usuarioId;
            orden.Estado = EstadoOrden.Asignada;

            _context.OrdenesRecogida.Update(orden);
            await _context.SaveChangesAsync();

            return await GetById(id);
        }

        public async Task<OrdenRecogidaDTO?> MarcarEnPreparacion(int id)
        {
            var orden = await _context.OrdenesRecogida
                .Include(o => o.PedidoVenta)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (orden == null || orden.Estado != EstadoOrden.Asignada){

                return null;
            }

            orden.Estado = EstadoOrden.EnPreparacion;

            if (orden.PedidoVenta != null) {

                orden.PedidoVenta.Estado = EstadoPedido.EnPreparacion;
            }

            await _context.SaveChangesAsync();

            return await GetById(id);
        }

        public async Task<OrdenRecogidaDTO?> Finalizar(int id, string usuarioId, FinalizarOrdenRecogidaDTO dto)
        {
            var orden = await _context.OrdenesRecogida
                .Include(o => o.PedidoVenta)
                    .ThenInclude(p => p!.Lineas)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (orden == null || orden.PedidoVenta == null){

                return null;
            }

            if (orden.Estado != EstadoOrden.Asignada && orden.Estado != EstadoOrden.EnPreparacion) {

                return null;
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                foreach (var linea in orden.PedidoVenta.Lineas){

                    var cantidadPendiente = linea.Cantidad;

                    var stocksAlmacen = await _context.StockUbicaciones.Include(s => s.Ubicacion).Where(s =>
                            s.ProductoId == linea.ProductoId &&
                            s.Ubicacion != null &&
                            s.Ubicacion.TipoUbicacion == TipoUbicacion.Almacen &&
                            s.Cantidad > 0).OrderBy(s => s.Ubicacion!.Codigo).ToListAsync();

                    var totalDisponible = stocksAlmacen.Sum(s => s.Cantidad);

                    if (totalDisponible < cantidadPendiente){

                        await transaction.RollbackAsync();
                        return null;
                    }

                    foreach (var stockOrigen in stocksAlmacen)
                    {
                        if (cantidadPendiente <= 0) {

                            break;
                        }

                        var cantidadDescontar = Math.Min(stockOrigen.Cantidad, cantidadPendiente);

                        stockOrigen.Cantidad -= cantidadDescontar;
                        cantidadPendiente -= cantidadDescontar;

                        await _context.MovimientosStock.AddAsync(new MovimientoStock
                        {
                            ProductoId = linea.ProductoId,
                            UbicacionOrigenId = stockOrigen.UbicacionId,
                            UbicacionDestinoId = null,
                            Cantidad = cantidadDescontar,
                            Fecha = DateTime.UtcNow,
                            UsuarioId = usuarioId,
                            TipoMovimiento = TipoMovimiento.PedidoCliente,
                            Observaciones = $"Orden de recogida finalizada. OrdenRecogidaId: {orden.Id}"
                        });
                    }
                }

                orden.UsuarioAsignadoId ??= usuarioId;
                orden.Estado = EstadoOrden.Finalizada;
                orden.Observaciones = dto.Observaciones ?? orden.Observaciones;

                orden.PedidoVenta.Estado = EstadoPedido.ListoRecogida;

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


        private async Task<OrdenRecogidaDTO?> MapearOrdenRecogidaCompleta(OrdenRecogida? orden)
        {

            if (orden == null) {

                return null;
            }

            ApplicationUser? usuarioAsignado = null;

            if (!string.IsNullOrEmpty(orden.UsuarioAsignadoId)){usuarioAsignado = await _context.Users.FirstOrDefaultAsync(u => u.Id == orden.UsuarioAsignadoId);}

            return ApplicationMapper.ToOrdenRecogidaDTO(orden, usuarioAsignado);
        }


        public async Task<PreparacionRecogidaDTO?> GetPreparacion(int id)
        {
            var orden = await _context.OrdenesRecogida.Include(o => o.PedidoVenta).ThenInclude(p => p!.Cliente).Include(o => o.PedidoVenta).ThenInclude(p => p!.Lineas).ThenInclude(l => l.Producto).FirstOrDefaultAsync(o => o.Id == id);

            if (orden == null || orden.PedidoVenta == null){

                return null;
            }

            if (orden.Estado == EstadoOrden.Finalizada || orden.Estado == EstadoOrden.Cancelada){

                return null;
            }

            var preparacion = new PreparacionRecogidaDTO
            {
                OrdenRecogidaId = orden.Id,
                PedidoVentaId = orden.PedidoVentaId,
                EstadoOrden = orden.Estado.ToString(),
                ClienteNombreCompleto = orden.PedidoVenta.Cliente == null ? string.Empty: $"{orden.PedidoVenta.Cliente.Nombre} {orden.PedidoVenta.Cliente.Apellidos}".Trim()
            };

            foreach (var linea in orden.PedidoVenta.Lineas)
            {
                var lineaPreparacion = new LineaPreparacionRecogidaDTO
                {
                    ProductoId = linea.ProductoId,
                    ProductoReferencia = linea.Producto?.Referencia ?? string.Empty,
                    ProductoNombre = linea.Producto?.Nombre ?? string.Empty,
                    CantidadSolicitada = linea.Cantidad
                };

                var cantidadPendiente = linea.Cantidad;

                var stocksAlmacen = await _context.StockUbicaciones.Include(s => s.Ubicacion).Where(s =>
                        s.ProductoId == linea.ProductoId &&
                        s.Ubicacion != null &&
                        s.Ubicacion.TipoUbicacion == TipoUbicacion.Almacen &&
                        s.Cantidad > 0).OrderBy(s => s.Ubicacion!.Codigo).ToListAsync();

                foreach (var stock in stocksAlmacen)
                {
                    if (cantidadPendiente <= 0){

                        break;
                    }

                    var cantidadARecoger = Math.Min(stock.Cantidad, cantidadPendiente);

                    lineaPreparacion.Ubicaciones.Add(new UbicacionPreparacionRecogidaDTO
                    {
                        UbicacionId = stock.UbicacionId,
                        UbicacionCodigo = stock.Ubicacion?.Codigo ?? string.Empty,
                        UbicacionNombre = stock.Ubicacion?.Nombre ?? string.Empty,
                        CantidadDisponible = stock.Cantidad,
                        CantidadARecoger = cantidadARecoger
                    });

                    cantidadPendiente -= cantidadARecoger;
                }

                preparacion.Lineas.Add(lineaPreparacion);
            }

            return preparacion;
        }

        public async Task<OrdenRecogidaDTO?> Cancelar(int id, string usuarioId, CancelarOrdenDTO dto)
        {
            var orden = await _context.OrdenesRecogida.Include(o => o.PedidoVenta).FirstOrDefaultAsync(o => o.Id == id);

            if (orden == null){
                return null;
            }

            if (orden.Estado != EstadoOrden.Asignada && orden.Estado != EstadoOrden.EnPreparacion){
                return null;
            }

            orden.Estado = EstadoOrden.Cancelada;
            orden.Observaciones = $"Cancelada: {dto.MotivoCancelacion}";

            if (orden.PedidoVenta != null){
                orden.PedidoVenta.Estado = EstadoPedido.Cancelado;
            }

            await _context.SaveChangesAsync();

            return await GetById(id);
        }
    }
}