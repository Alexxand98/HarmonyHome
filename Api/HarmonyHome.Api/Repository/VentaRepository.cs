using HarmonyHome.Api.Data;
using HarmonyHome.Api.Helpers;
using HarmonyHome.Api.Models.DTOs;
using HarmonyHome.Api.Models.DTOs.VentaMixtaDto;
using HarmonyHome.Api.Models.Entity;
using HarmonyHome.Api.Models.Enums;
using HarmonyHome.Api.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace HarmonyHome.Api.Repository
{
    public class VentaRepository : IVentaRepository
    {
        private readonly ApplicationDbContext _context;

        public VentaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<VentaDTO?> CrearVentaDirecta(CreateVentaDirectaDTO createVentaDirectaDTO, string usuarioId)
        {
            var clienteExiste = await _context.Clientes
                .AnyAsync(c => c.Id == createVentaDirectaDTO.ClienteId && c.Activo);

            if (!clienteExiste)
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
                foreach (var linea in createVentaDirectaDTO.Lineas)
                {
                    var producto = await _context.Productos
                        .FirstOrDefaultAsync(p => p.Id == linea.ProductoId && p.Activo && p.Habilitado);

                    if (producto == null)
                    {
                        await transaction.RollbackAsync();
                        return null;
                    }

                    var stock = await _context.StockUbicaciones
                        .FirstOrDefaultAsync(s =>
                            s.ProductoId == linea.ProductoId &&
                            s.UbicacionId == ubicacionTienda.Id);

                    if (stock == null || stock.Cantidad < linea.Cantidad)
                    {
                        await transaction.RollbackAsync();
                        return null;
                    }
                }

                var pedidoVenta = new PedidoVenta
                {
                    ClienteId = createVentaDirectaDTO.ClienteId,
                    UsuarioId = usuarioId,
                    FechaCreacion = DateTime.UtcNow,
                    Estado = EstadoPedido.Entregado,
                    TipoPedidoVenta = TipoPedidoVenta.VentaDirecta,
                    Observaciones = createVentaDirectaDTO.Observaciones
                };

                decimal total = 0;

                foreach (var lineaDto in createVentaDirectaDTO.Lineas)
                {
                    var producto = await _context.Productos
                        .FirstAsync(p => p.Id == lineaDto.ProductoId);

                    var subtotal = lineaDto.Cantidad * producto.PrecioVenta;
                    total += subtotal;

                    var lineaPedido = new LineaPedidoVenta
                    {
                        PedidoVenta = pedidoVenta,
                        ProductoId = producto.Id,
                        Cantidad = lineaDto.Cantidad,
                        PrecioUnitario = producto.PrecioVenta,
                        Subtotal = subtotal
                    };

                    pedidoVenta.Lineas.Add(lineaPedido);

                    var stock = await _context.StockUbicaciones
                        .FirstAsync(s =>
                            s.ProductoId == producto.Id &&
                            s.UbicacionId == ubicacionTienda.Id);

                    stock.Cantidad -= lineaDto.Cantidad;

                    var movimiento = new MovimientoStock
                    {
                        ProductoId = producto.Id,
                        UbicacionOrigenId = ubicacionTienda.Id,
                        UbicacionDestinoId = null,
                        Cantidad = lineaDto.Cantidad,
                        Fecha = DateTime.UtcNow,
                        UsuarioId = usuarioId,
                        TipoMovimiento = TipoMovimiento.VentaDirecta,
                        Observaciones = $"Venta directa. PedidoVenta pendiente de guardar."
                    };

                    await _context.MovimientosStock.AddAsync(movimiento);
                }

                pedidoVenta.Total = total;

                await _context.PedidosVenta.AddAsync(pedidoVenta);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                var venta = await _context.PedidosVenta
                    .Include(p => p.Cliente)
                    .Include(p => p.Lineas)
                        .ThenInclude(l => l.Producto)
                    .FirstOrDefaultAsync(p => p.Id == pedidoVenta.Id);

                return venta == null ? null : ApplicationMapper.ToVentaDTO(venta);
            }
            catch
            {
                await transaction.RollbackAsync();
                return null;
            }
        }


        public async Task<VentaMixtaDTO?> CrearVentaMixta(CreateVentaMixtaDTO dto, string usuarioId)
        {
            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.Id == dto.ClienteId && c.Activo);

            if (cliente == null){

                return null;
            }

            if (dto.Lineas == null || !dto.Lineas.Any()){

                return null;
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var lineasVentaDirecta = new List<LineaPedidoVenta>();
                var lineasPedidoCliente = new List<LineaPedidoVenta>();
                var resultado = new VentaMixtaDTO();

                foreach (var lineaDto in dto.Lineas){

                    if (lineaDto.Cantidad <= 0){

                        await transaction.RollbackAsync();
                        return null;
                    }

                    var producto = await _context.Productos.FirstOrDefaultAsync(p => p.Id == lineaDto.ProductoId && p.Activo && p.Habilitado);

                    if (producto == null) {

                        await transaction.RollbackAsync();
                        return null;
                    }

                    var stockTienda = await _context.StockUbicaciones.Include(s => s.Ubicacion).Where(s =>
                            s.ProductoId == producto.Id &&
                            s.Ubicacion != null &&
                            s.Ubicacion.TipoUbicacion == TipoUbicacion.Tienda).SumAsync(s => s.Cantidad);

                    var stockAlmacen = await _context.StockUbicaciones.Include(s => s.Ubicacion).Where(s =>
                            s.ProductoId == producto.Id &&
                            s.Ubicacion != null &&
                            s.Ubicacion.TipoUbicacion == TipoUbicacion.Almacen).SumAsync(s => s.Cantidad);

                    var cantidadSolicitada = lineaDto.Cantidad;

                    if (stockTienda + stockAlmacen < cantidadSolicitada){

                        await transaction.RollbackAsync();
                        return null;
                    }

                    var cantidadVentaDirecta = 0;

                    var cantidadPedidoCliente = 0;

                    if (stockTienda >= cantidadSolicitada)  {

                        cantidadVentaDirecta = cantidadSolicitada;

                    }else if (stockAlmacen >= cantidadSolicitada){

                        cantidadPedidoCliente = cantidadSolicitada;

                    }else {

                        cantidadVentaDirecta = stockTienda;

                        cantidadPedidoCliente = cantidadSolicitada - stockTienda;
                    }

                    if (cantidadVentaDirecta > 0){
                        lineasVentaDirecta.Add(new LineaPedidoVenta
                        {
                            ProductoId = producto.Id,
                            Cantidad = cantidadVentaDirecta,
                            PrecioUnitario = producto.PrecioVenta,
                            Subtotal = cantidadVentaDirecta * producto.PrecioVenta
                        });
                    }

                    if (cantidadPedidoCliente > 0){
                        lineasPedidoCliente.Add(new LineaPedidoVenta
                        {
                            ProductoId = producto.Id,
                            Cantidad = cantidadPedidoCliente,
                            PrecioUnitario = producto.PrecioVenta,
                            Subtotal = cantidadPedidoCliente * producto.PrecioVenta
                        });
                    }

                    resultado.Lineas.Add(new LineaVentaMixtaResultadoDTO
                    {
                        ProductoId = producto.Id,
                        ProductoReferencia = producto.Referencia,
                        ProductoNombre = producto.Nombre,
                        CantidadSolicitada = cantidadSolicitada,
                        CantidadVentaDirecta = cantidadVentaDirecta,
                        CantidadPedidoCliente = cantidadPedidoCliente,
                        PrecioUnitario = producto.PrecioVenta,
                        SubtotalVentaDirecta = cantidadVentaDirecta * producto.PrecioVenta,
                        SubtotalPedidoCliente = cantidadPedidoCliente * producto.PrecioVenta
                    });
                }

                PedidoVenta? ventaDirecta = null;

                if (lineasVentaDirecta.Any()) {
                    ventaDirecta = new PedidoVenta
                    {
                        ClienteId = dto.ClienteId,
                        UsuarioId = usuarioId,
                        FechaCreacion = DateTime.UtcNow,
                        Estado = EstadoPedido.Entregado,
                        TipoPedidoVenta = TipoPedidoVenta.VentaDirecta,
                        Observaciones = dto.Observaciones,
                        Total = lineasVentaDirecta.Sum(l => l.Subtotal)
                    };

                    foreach (var linea in lineasVentaDirecta){

                        ventaDirecta.Lineas.Add(linea);
                    }

                    await _context.PedidosVenta.AddAsync(ventaDirecta);

                    await _context.SaveChangesAsync();

                    foreach (var linea in lineasVentaDirecta) {
                        var cantidadPendiente = linea.Cantidad;

                        var stocksTienda = await _context.StockUbicaciones.Include(s => s.Ubicacion).Where(s =>
                                s.ProductoId == linea.ProductoId &&
                                s.Ubicacion != null &&
                                s.Ubicacion.TipoUbicacion == TipoUbicacion.Tienda &&
                                s.Cantidad > 0).OrderBy(s => s.Ubicacion!.Codigo).ToListAsync();

                        foreach (var stock in stocksTienda)  {

                            if (cantidadPendiente <= 0) {

                                break;
                            }

                            var cantidadDescontar = Math.Min(stock.Cantidad, cantidadPendiente);

                            stock.Cantidad -= cantidadDescontar;

                            cantidadPendiente -= cantidadDescontar;

                            await _context.MovimientosStock.AddAsync(new MovimientoStock
                            {
                                ProductoId = linea.ProductoId,
                                UbicacionOrigenId = stock.UbicacionId,
                                UbicacionDestinoId = null,
                                Cantidad = cantidadDescontar,
                                Fecha = DateTime.UtcNow,
                                UsuarioId = usuarioId,
                                TipoMovimiento = TipoMovimiento.VentaDirecta,
                                Observaciones = $"Venta mixta - parte venta directa. PedidoVentaId: {ventaDirecta.Id}"
                            });
                        }
                    }

                    resultado.VentaDirectaId = ventaDirecta.Id;
                    resultado.TotalVentaDirecta = ventaDirecta.Total;
                }

                PedidoVenta? pedidoCliente = null;
                OrdenRecogida? ordenRecogida = null;

                if (lineasPedidoCliente.Any()) {
                    pedidoCliente = new PedidoVenta
                    {
                        ClienteId = dto.ClienteId,
                        UsuarioId = usuarioId,
                        FechaCreacion = DateTime.UtcNow,
                        Estado = EstadoPedido.Pendiente,
                        TipoPedidoVenta = TipoPedidoVenta.PedidoCliente,
                        Observaciones = dto.Observaciones,
                        Total = lineasPedidoCliente.Sum(l => l.Subtotal)
                    };

                    foreach (var linea in lineasPedidoCliente) {
                        pedidoCliente.Lineas.Add(linea);
                    }

                    ordenRecogida = new OrdenRecogida
                    {
                        FechaCreacion = DateTime.UtcNow,
                        Estado = EstadoOrden.Pendiente,
                        Observaciones = "Orden generada automáticamente desde venta mixta",
                        PedidoVenta = pedidoCliente
                    };

                    await _context.OrdenesRecogida.AddAsync(ordenRecogida);
                    await _context.SaveChangesAsync();

                    resultado.PedidoClienteId = pedidoCliente.Id;
                    resultado.OrdenRecogidaId = ordenRecogida.Id;
                    resultado.TotalPedidoCliente = pedidoCliente.Total;
                }

                if (resultado.VentaDirectaId != null && resultado.PedidoClienteId != null) {

                    resultado.TipoOperacion = "Mixta";

                }else if (resultado.VentaDirectaId != null){

                    resultado.TipoOperacion = "VentaDirecta";

                }else {

                    resultado.TipoOperacion = "PedidoCliente";
                }

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return resultado;

            } catch{

                await transaction.RollbackAsync();

                return null;
            }
        }
    }
}