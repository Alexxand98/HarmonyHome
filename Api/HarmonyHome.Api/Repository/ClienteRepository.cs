using HarmonyHome.Api.Data;
using HarmonyHome.Api.Helpers;
using HarmonyHome.Api.Models.DTOs;
using HarmonyHome.Api.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace HarmonyHome.Api.Repository
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly ApplicationDbContext _context;

        public ClienteRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ClienteDTO>> GetAll()
        {
            return await _context.Clientes
                .OrderBy(c => c.Nombre)
                .Select(c => ApplicationMapper.ToClienteDTO(c))
                .ToListAsync();
        }

        public async Task<List<ClienteDTO>> GetActivos()
        {
            return await _context.Clientes
                .Where(c => c.Activo)
                .OrderBy(c => c.Nombre)
                .Select(c => ApplicationMapper.ToClienteDTO(c))
                .ToListAsync();
        }

        public async Task<ClienteDTO?> GetById(int id)
        {
            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(c => c.Id == id);

            return cliente == null ? null : ApplicationMapper.ToClienteDTO(cliente);
        }

        public async Task<List<ClienteDTO>> Buscar(string? texto)
        {
            var query = _context.Clientes.AsQueryable();

            if (!string.IsNullOrWhiteSpace(texto))
            {
                var textoLower = texto.ToLower();

                query = query.Where(c =>
                    c.Nombre.ToLower().Contains(textoLower) ||
                    (c.Apellidos != null && c.Apellidos.ToLower().Contains(textoLower)) ||
                    (c.Telefono != null && c.Telefono.ToLower().Contains(textoLower)) ||
                    (c.Email != null && c.Email.ToLower().Contains(textoLower)));
            }

            return await query
                .OrderBy(c => c.Nombre)
                .Select(c => ApplicationMapper.ToClienteDTO(c))
                .ToListAsync();
        }

        public async Task<ClienteDTO?> Create(CreateClienteDTO createClienteDTO)
        {
            var cliente = ApplicationMapper.ToCliente(createClienteDTO);

            await _context.Clientes.AddAsync(cliente);
            await _context.SaveChangesAsync();

            return ApplicationMapper.ToClienteDTO(cliente);
        }

        public async Task<ClienteDTO?> Update(int id, UpdateClienteDTO updateClienteDTO)
        {
            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cliente == null)
            {
                return null;
            }

            ApplicationMapper.UpdateCliente(cliente, updateClienteDTO);

            _context.Clientes.Update(cliente);
            await _context.SaveChangesAsync();

            return ApplicationMapper.ToClienteDTO(cliente);
        }

        public async Task<bool> Delete(int id)
        {
            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cliente == null)
            {
                return false;
            }

            cliente.Activo = false;

            _context.Clientes.Update(cliente);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}