using HarmonyHome.Api.Data;
using HarmonyHome.Api.Helpers;
using HarmonyHome.Api.Models.DTOs;
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
            return await _context.OrdenesRecogida
                .Include(o => o.PedidoVenta)
                    .ThenInclude(p => p!.Cliente)
                .Include(o => o.PedidoVenta)
                    .ThenInclude(p => p!.Lineas)
                        .ThenInclude(l => l.Producto)
                .OrderByDescending(o => o.FechaCreacion)
                .Select(o => ApplicationMapper.ToOrdenRecogidaDTO(o))
                .ToListAsync();
        }

        public async Task<List<OrdenRecogidaDTO>> GetPendientes()
        {
            return await _context.OrdenesRecogida
                .Include(o => o.PedidoVenta)
                    .ThenInclude(p => p!.Cliente)
                .Include(o => o.PedidoVenta)
                    .ThenInclude(p => p!.Lineas)
                        .ThenInclude(l => l.Producto)
                .Where(o => o.Estado == EstadoOrden.Pendiente)
                .OrderBy(o => o.FechaCreacion)
                .Select(o => ApplicationMapper.ToOrdenRecogidaDTO(o))
                .ToListAsync();
        }

        public async Task<OrdenRecogidaDTO?> GetById(int id)
        {
            var orden = await _context.OrdenesRecogida
                .Include(o => o.PedidoVenta)
                    .ThenInclude(p => p!.Cliente)
                .Include(o => o.PedidoVenta)
                    .ThenInclude(p => p!.Lineas)
                        .ThenInclude(l => l.Producto)
                .FirstOrDefaultAsync(o => o.Id == id);

            return orden == null ? null : ApplicationMapper.ToOrdenRecogidaDTO(orden);
        }

        public async Task<OrdenRecogidaDTO?> Asignar(int id, string usuarioId)
        {
            var orden = await _context.OrdenesRecogida
                .FirstOrDefaultAsync(o => o.Id == id);

            if (orden == null || orden.Estado != EstadoOrden.Pendiente)
            {
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

            if (orden == null || orden.Estado != EstadoOrden.Asignada)
            {
                return null;
            }

            orden.Estado = EstadoOrden.EnPreparacion;

            if (orden.PedidoVenta != null)
            {
                orden.PedidoVenta.Estado = EstadoPedido.EnPreparacion;
            }

            await _context.SaveChangesAsync();

            return await GetById(id);
        }

        public async Task<OrdenRecogidaDTO?> Finalizar(int id, string usuarioId, FinalizarOrdenRecogidaDTO dto)
        {
            var orden = await _context.OrdenesRecogida
                .Include(o => o.PedidoVenta)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (orden == null)
            {
                return null;
            }

            if (orden.Estado != EstadoOrden.Asignada && orden.Estado != EstadoOrden.EnPreparacion)
            {
                return null;
            }

            orden.UsuarioAsignadoId ??= usuarioId;
            orden.Estado = EstadoOrden.Finalizada;
            orden.Observaciones = dto.Observaciones ?? orden.Observaciones;

            if (orden.PedidoVenta != null)
            {
                orden.PedidoVenta.Estado = EstadoPedido.ListoRecogida;
            }

            await _context.SaveChangesAsync();

            return await GetById(id);
        }
    }
}