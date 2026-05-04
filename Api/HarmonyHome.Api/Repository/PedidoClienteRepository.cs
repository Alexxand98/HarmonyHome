using HarmonyHome.Api.Data;
using HarmonyHome.Api.Helpers;
using HarmonyHome.Api.Models.DTOs;
using HarmonyHome.Api.Models.Entity;
using HarmonyHome.Api.Models.Enums;
using HarmonyHome.Api.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace HarmonyHome.Api.Repository
{
    public class PedidoClienteRepository : IPedidoClienteRepository
    {
        private readonly ApplicationDbContext _context;

        public PedidoClienteRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PedidoClienteDTO?> CrearPedidoCliente(CreatePedidoClienteDTO createPedidoClienteDTO, string usuarioId)
        {
            var clienteExiste = await _context.Clientes
                .AnyAsync(c => c.Id == createPedidoClienteDTO.ClienteId && c.Activo);

            if (!clienteExiste)
            {
                return null;
            }

            var hayAlmacen = await _context.Ubicaciones
                .AnyAsync(u => u.TipoUbicacion == TipoUbicacion.Almacen && u.Activa);

            if (!hayAlmacen)
            {
                return null;
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                foreach (var linea in createPedidoClienteDTO.Lineas)
                {
                    var producto = await _context.Productos
                        .FirstOrDefaultAsync(p => p.Id == linea.ProductoId && p.Activo && p.Habilitado);

                    if (producto == null)
                    {
                        await transaction.RollbackAsync();
                        return null;
                    }

                    var stockAlmacen = await _context.StockUbicaciones
                        .Include(s => s.Ubicacion)
                        .Where(s =>
                            s.ProductoId == linea.ProductoId &&
                            s.Ubicacion != null &&
                            s.Ubicacion.TipoUbicacion == TipoUbicacion.Almacen)
                        .SumAsync(s => s.Cantidad);

                    if (stockAlmacen < linea.Cantidad)
                    {
                        await transaction.RollbackAsync();
                        return null;
                    }
                }

                var pedidoVenta = new PedidoVenta
                {
                    ClienteId = createPedidoClienteDTO.ClienteId,
                    UsuarioId = usuarioId,
                    FechaCreacion = DateTime.UtcNow,
                    Estado = EstadoPedido.Pendiente,
                    TipoPedidoVenta = TipoPedidoVenta.PedidoCliente,
                    Observaciones = createPedidoClienteDTO.Observaciones
                };

                decimal total = 0;

                foreach (var lineaDto in createPedidoClienteDTO.Lineas)
                {
                    var producto = await _context.Productos
                        .FirstAsync(p => p.Id == lineaDto.ProductoId);

                    var subtotal = lineaDto.Cantidad * producto.PrecioVenta;
                    total += subtotal;

                    pedidoVenta.Lineas.Add(new LineaPedidoVenta
                    {
                        PedidoVenta = pedidoVenta,
                        ProductoId = producto.Id,
                        Cantidad = lineaDto.Cantidad,
                        PrecioUnitario = producto.PrecioVenta,
                        Subtotal = subtotal
                    });
                }

                pedidoVenta.Total = total;

                var ordenRecogida = new OrdenRecogida
                {
                    PedidoVenta = pedidoVenta,
                    FechaCreacion = DateTime.UtcNow,
                    Estado = EstadoOrden.Pendiente,
                    UsuarioAsignadoId = null,
                    Observaciones = "Orden generada automáticamente desde pedido de cliente."
                };

                await _context.PedidosVenta.AddAsync(pedidoVenta);
                await _context.OrdenesRecogida.AddAsync(ordenRecogida);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                var pedidoGuardado = await _context.PedidosVenta
                    .Include(p => p.Cliente)
                    .Include(p => p.Lineas)
                        .ThenInclude(l => l.Producto)
                    .FirstOrDefaultAsync(p => p.Id == pedidoVenta.Id);

                var ordenGuardada = await _context.OrdenesRecogida.FirstOrDefaultAsync(o => o.PedidoVentaId == pedidoVenta.Id);

                return pedidoGuardado == null ? null : ApplicationMapper.ToPedidoClienteDTO(pedidoGuardado, ordenGuardada);
            }
            catch
            {
                await transaction.RollbackAsync();
                return null;
            }
        }
    }
}