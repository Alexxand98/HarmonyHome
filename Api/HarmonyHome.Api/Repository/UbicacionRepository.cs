using HarmonyHome.Api.Data;
using HarmonyHome.Api.Helpers;
using HarmonyHome.Api.Models.DTO;
using HarmonyHome.Api.Models.DTOs;
using HarmonyHome.Api.Models.Enums;
using HarmonyHome.Api.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace HarmonyHome.Api.Repository
{
    public class UbicacionRepository : IUbicacionRepository
    {
        private readonly ApplicationDbContext _context;

        public UbicacionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<UbicacionDTO>> GetAll()
        {
            return await _context.Ubicaciones
                .OrderBy(u => u.Codigo)
                .Select(u => ApplicationMapper.ToUbicacionDTO(u))
                .ToListAsync();
        }

        public async Task<List<UbicacionDTO>> GetActivas()
        {
            return await _context.Ubicaciones
                .Where(u => u.Activa)
                .OrderBy(u => u.Codigo)
                .Select(u => ApplicationMapper.ToUbicacionDTO(u))
                .ToListAsync();
        }

        public async Task<List<UbicacionDTO>> GetByTipo(TipoUbicacion tipoUbicacion)
        {
            return await _context.Ubicaciones
                .Where(u => u.TipoUbicacion == tipoUbicacion && u.Activa)
                .OrderBy(u => u.Codigo)
                .Select(u => ApplicationMapper.ToUbicacionDTO(u))
                .ToListAsync();
        }

        public async Task<UbicacionDTO?> GetById(int id)
        {
            var ubicacion = await _context.Ubicaciones
                .FirstOrDefaultAsync(u => u.Id == id);

            return ubicacion == null ? null : ApplicationMapper.ToUbicacionDTO(ubicacion);
        }

        public async Task<UbicacionDTO?> GetByCodigo(string codigo)
        {
            var ubicacion = await _context.Ubicaciones
                .FirstOrDefaultAsync(u => u.Codigo.ToLower() == codigo.ToLower());

            return ubicacion == null ? null : ApplicationMapper.ToUbicacionDTO(ubicacion);
        }

        public async Task<UbicacionDTO?> Create(CreateUbicacionDTO createUbicacionDTO)
        {
            if (await ExisteCodigo(createUbicacionDTO.Codigo))
            {
                return null;
            }

            var ubicacion = ApplicationMapper.ToUbicacion(createUbicacionDTO);

            await _context.Ubicaciones.AddAsync(ubicacion);
            await _context.SaveChangesAsync();

            return ApplicationMapper.ToUbicacionDTO(ubicacion);
        }

        public async Task<UbicacionDTO?> Update(int id, UpdateUbicacionDTO updateUbicacionDTO)
        {
            var ubicacion = await _context.Ubicaciones
                .FirstOrDefaultAsync(u => u.Id == id);

            if (ubicacion == null)
            {
                return null;
            }

            if (await ExisteCodigoEnOtraUbicacion(updateUbicacionDTO.Codigo, id))
            {
                return null;
            }

            ApplicationMapper.UpdateUbicacion(ubicacion, updateUbicacionDTO);

            _context.Ubicaciones.Update(ubicacion);
            await _context.SaveChangesAsync();

            return ApplicationMapper.ToUbicacionDTO(ubicacion);
        }

        public async Task<bool> Delete(int id)
        {
            var ubicacion = await _context.Ubicaciones
                .FirstOrDefaultAsync(u => u.Id == id);

            if (ubicacion == null)
            {
                return false;
            }

            var tieneStock = await _context.StockUbicaciones
                .AnyAsync(s => s.UbicacionId == id && s.Cantidad > 0);

            if (tieneStock)
            {
                return false;
            }

            ubicacion.Activa = false;

            _context.Ubicaciones.Update(ubicacion);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ExisteCodigo(string codigo)
        {
            return await _context.Ubicaciones
                .AnyAsync(u => u.Codigo.ToLower() == codigo.ToLower());
        }

        public async Task<bool> ExisteCodigoEnOtraUbicacion(string codigo, int id)
        {
            return await _context.Ubicaciones
                .AnyAsync(u => u.Id != id && u.Codigo.ToLower() == codigo.ToLower());
        }
    }
}