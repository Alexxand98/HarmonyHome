using HarmonyHome.Api.Data;
using HarmonyHome.Api.Models.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HarmonyHome.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductoController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Producto>>> GetProductos()
        {
            var productos = await _context.Productos.ToListAsync();

            return Ok(productos);
        }

        [HttpPost]
        public async Task<ActionResult<Producto>> CrearProducto([FromBody] Producto producto)
        {
            if (producto == null)
            {
                return BadRequest("El producto no puede ser nulo.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _context.Productos.AddAsync(producto);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProductos), new { id = producto.Id }, producto);
        }
    }
}