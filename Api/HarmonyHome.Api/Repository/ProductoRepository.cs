using HarmonyHome.Api.Data;
using HarmonyHome.Api.Helpers;
using HarmonyHome.Api.Models.DTOs;
using HarmonyHome.Api.Models.Entity;
using HarmonyHome.Api.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace HarmonyHome.Api.Repository
{
    public class ProductoRepository : IProductoRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProductoDTO>> GetAll()
        {
            return await _context.Productos
                .OrderBy(p => p.Nombre)
                .Select(p => ApplicationMapper.ToProductoDTO(p))
                .ToListAsync();
        }

        public async Task<List<ProductoDTO>> GetHabilitados()
        {
            return await _context.Productos
                .Where(p => p.Activo && p.Habilitado)
                .OrderBy(p => p.Nombre)
                .Select(p => ApplicationMapper.ToProductoDTO(p))
                .ToListAsync();
        }

        public async Task<ProductoDTO?> GetById(int id)
        {
            var producto = await _context.Productos
                .FirstOrDefaultAsync(p => p.Id == id);

            if (producto == null)
            {
                return null;
            }

            return ApplicationMapper.ToProductoDTO(producto);
        }

        public async Task<ProductoDTO?> GetByReferencia(string referencia)
        {
            var producto = await _context.Productos
                .FirstOrDefaultAsync(p => p.Referencia.ToLower() == referencia.ToLower());

            if (producto == null)
            {
                return null;
            }

            return ApplicationMapper.ToProductoDTO(producto);
        }

        public async Task<List<ProductoDTO>> Buscar(string? texto, string? categoria)
        {
            var query = _context.Productos.AsQueryable();

            if (!string.IsNullOrWhiteSpace(texto))
            {
                var textoLower = texto.ToLower();

                query = query.Where(p =>
                    p.Referencia.ToLower().Contains(textoLower) ||
                    p.Nombre.ToLower().Contains(textoLower));
            }

            if (!string.IsNullOrWhiteSpace(categoria))
            {
                var categoriaLower = categoria.ToLower();

                query = query.Where(p =>
                    p.Categoria.ToLower().Contains(categoriaLower));
            }

            return await query
                .OrderBy(p => p.Nombre)
                .Select(p => ApplicationMapper.ToProductoDTO(p))
                .ToListAsync();
        }

        public async Task<ProductoDTO?> Create(CreateProductoDTO createProductoDTO)
        {
            if (await ExisteReferencia(createProductoDTO.Referencia))
            {
                return null;
            }

            var producto = ApplicationMapper.ToProducto(createProductoDTO);

            await _context.Productos.AddAsync(producto);
            await _context.SaveChangesAsync();

            return ApplicationMapper.ToProductoDTO(producto);
        }

        public async Task<ProductoDTO?> Update(int id, UpdateProductoDTO updateProductoDTO)
        {
            var producto = await _context.Productos
                .FirstOrDefaultAsync(p => p.Id == id);

            if (producto == null)
            {
                return null;
            }

            if (await ExisteReferenciaEnOtroProducto(updateProductoDTO.Referencia, id))
            {
                return null;
            }

            ApplicationMapper.UpdateProducto(producto, updateProductoDTO);

            _context.Productos.Update(producto);
            await _context.SaveChangesAsync();

            return ApplicationMapper.ToProductoDTO(producto);
        }

        public async Task<string?> Delete(int id)
        {
            var producto = await _context.Productos
                .FirstOrDefaultAsync(p => p.Id == id);

            if (producto == null)
            {
                return null;
            }

            var tieneRelaciones =
                await _context.StockUbicaciones.AnyAsync(s => s.ProductoId == id) ||
                await _context.MovimientosStock.AnyAsync(m => m.ProductoId == id) ||
                await _context.LineasPedidoVenta.AnyAsync(l => l.ProductoId == id) ||
                await _context.LineasOrdenReposicion.AnyAsync(l => l.ProductoId == id);

            if (!tieneRelaciones)
            {
                _context.Productos.Remove(producto);
                await _context.SaveChangesAsync();

                return "Producto eliminado correctamente.";
            }

            producto.Activo = false;
            producto.Habilitado = false;

            _context.Productos.Update(producto);
            await _context.SaveChangesAsync();

            return "El producto tiene datos asociados, baja lógica realizada correctamente";
        }

        public async Task<bool> Habilitar(int id)
        {
            var producto = await _context.Productos
                .FirstOrDefaultAsync(p => p.Id == id);

            if (producto == null || !producto.Activo)
            {
                return false;
            }

            producto.Habilitado = true;

            _context.Productos.Update(producto);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> Deshabilitar(int id)
        {
            var producto = await _context.Productos
                .FirstOrDefaultAsync(p => p.Id == id);

            if (producto == null)
            {
                return false;
            }

            producto.Habilitado = false;

            _context.Productos.Update(producto);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ExisteReferencia(string referencia)
        {
            return await _context.Productos
                .AnyAsync(p => p.Referencia.ToLower() == referencia.ToLower());
        }

        public async Task<bool> ExisteReferenciaEnOtroProducto(string referencia, int id)
        {
            return await _context.Productos
                .AnyAsync(p => p.Id != id && p.Referencia.ToLower() == referencia.ToLower());
        }
    }
}