using HarmonyHome.Api.Data;
using HarmonyHome.Api.Helpers;
using HarmonyHome.Api.Models.DTOs;
using HarmonyHome.Api.Models.Entity;
using HarmonyHome.Api.Models.Enums;
using HarmonyHome.Api.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace HarmonyHome.Api.Repository
{
    public class DemarcaRepository : IDemarcaRepository
    {
        private readonly ApplicationDbContext _context;

        public DemarcaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DemarcaDTO?> CrearDemarca(CreateDemarcaDTO dto, string usuarioId, string rolUsuario)
        {
            var producto = await _context.Productos.FirstOrDefaultAsync(p => p.Id == dto.ProductoId && p.Activo);

            if (producto == null)
            {
                return null;
            }

            var ubicacion = await _context.Ubicaciones.FirstOrDefaultAsync(u => u.Id == dto.UbicacionId && u.Activa);

            if (ubicacion == null)
            {
                return null;
            }


            if (rolUsuario == "Vendedor" && ubicacion.TipoUbicacion != TipoUbicacion.Tienda)
            {
                return null;
            }


            if (rolUsuario == "Logistico" && ubicacion.TipoUbicacion == TipoUbicacion.Tienda)
            {
                return null;
            }

            var stock = await _context.StockUbicaciones
                .FirstOrDefaultAsync(s =>
                    s.ProductoId == dto.ProductoId &&
                    s.UbicacionId == dto.UbicacionId);

            if (stock == null || stock.Cantidad < dto.Cantidad)
            {
                return null;
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                stock.Cantidad -= dto.Cantidad;

                var movimiento = new MovimientoStock
                {
                    ProductoId = dto.ProductoId,
                    UbicacionOrigenId = dto.UbicacionId,

                    UbicacionDestinoId = null,
                    Cantidad = dto.Cantidad,

                    Fecha = DateTime.UtcNow,
                    UsuarioId = usuarioId,

                    TipoMovimiento = TipoMovimiento.Demarca,
                    Observaciones = dto.Motivo
                };

                await _context.MovimientosStock.AddAsync(movimiento);

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                var movimientoGuardado = await _context.MovimientosStock
                    .Include(m => m.Producto)
                    .Include(m => m.UbicacionOrigen)
                    .FirstOrDefaultAsync(m => m.Id == movimiento.Id);

                return movimientoGuardado == null ? null : ApplicationMapper.ToDemarcaDTO(movimientoGuardado);
            }
            catch
            {
                await transaction.RollbackAsync();

                return null;
            }
        }
    }
}