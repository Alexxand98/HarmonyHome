using HarmonyHome.Api.Data;
using HarmonyHome.Api.Helpers;
using HarmonyHome.Api.Models.DTOs;
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
    }
}