using System.Net;
using HarmonyHome.Api.Models.DTOs;
using HarmonyHome.Api.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HarmonyHome.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductoController : ControllerBase
    {
        private readonly IProductoRepository _productoRepository;
        private readonly ResponseApi _responseApi;

        public ProductoController(IProductoRepository productoRepository)
        {
            _productoRepository = productoRepository;
            _responseApi = new ResponseApi();
        }

        [HttpGet]
        [Authorize(Roles = "SupervisorLogistico,Administrador")]
        public async Task<IActionResult> GetProductos()
        {
            var productos = await _productoRepository.GetAll();

            _responseApi.StatusCode = HttpStatusCode.OK;
            _responseApi.IsSuccess = true;
            _responseApi.Result = productos;

            return Ok(_responseApi);
        }

        [HttpGet("habilitados")]
        [Authorize(Roles = "Vendedor,EncargadoTienda,SupervisorLogistico,Administrador")]
        public async Task<IActionResult> GetProductosHabilitados()
        {
            var productos = await _productoRepository.GetHabilitados();

            _responseApi.StatusCode = HttpStatusCode.OK;
            _responseApi.IsSuccess = true;
            _responseApi.Result = productos;

            return Ok(_responseApi);
        }

        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<IActionResult> GetProducto(int id)
        {
            var producto = await _productoRepository.GetById(id);

            if (producto == null)
            {
                _responseApi.StatusCode = HttpStatusCode.NotFound;
                _responseApi.IsSuccess = false;
                _responseApi.ErrorMessages.Add("Producto no encontrado.");

                return NotFound(_responseApi);
            }

            _responseApi.StatusCode = HttpStatusCode.OK;
            _responseApi.IsSuccess = true;
            _responseApi.Result = producto;

            return Ok(_responseApi);
        }

        [HttpGet("referencia/{referencia}")]
        [Authorize]
        public async Task<IActionResult> GetProductoPorReferencia(string referencia)
        {
            var producto = await _productoRepository.GetByReferencia(referencia);

            if (producto == null)
            {
                _responseApi.StatusCode = HttpStatusCode.NotFound;
                _responseApi.IsSuccess = false;
                _responseApi.ErrorMessages.Add("Producto no encontrado.");

                return NotFound(_responseApi);
            }

            _responseApi.StatusCode = HttpStatusCode.OK;
            _responseApi.IsSuccess = true;
            _responseApi.Result = producto;

            return Ok(_responseApi);
        }

        [HttpGet("buscar")]
        [Authorize]
        public async Task<IActionResult> BuscarProductos([FromQuery] string? texto, [FromQuery] string? categoria)
        {
            var productos = await _productoRepository.Buscar(texto, categoria);

            _responseApi.StatusCode = HttpStatusCode.OK;
            _responseApi.IsSuccess = true;
            _responseApi.Result = productos;

            return Ok(_responseApi);
        }

        [HttpPost]
        [Authorize(Roles = "SupervisorLogistico,Administrador")]
        public async Task<IActionResult> CrearProducto([FromBody] CreateProductoDTO createProductoDTO)
        {
            if (!ModelState.IsValid)
            {
                _responseApi.StatusCode = HttpStatusCode.BadRequest;
                _responseApi.IsSuccess = false;
                _responseApi.ErrorMessages = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(_responseApi);
            }

            var producto = await _productoRepository.Create(createProductoDTO);

            if (producto == null)
            {
                _responseApi.StatusCode = HttpStatusCode.BadRequest;
                _responseApi.IsSuccess = false;
                _responseApi.ErrorMessages.Add("Ya existe un producto con esa referencia.");

                return BadRequest(_responseApi);
            }

            _responseApi.StatusCode = HttpStatusCode.Created;
            _responseApi.IsSuccess = true;
            _responseApi.Result = producto;

            return StatusCode(StatusCodes.Status201Created, _responseApi);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "SupervisorLogistico,Administrador")]
        public async Task<IActionResult> ActualizarProducto(int id, [FromBody] UpdateProductoDTO updateProductoDTO)
        {
            if (!ModelState.IsValid)
            {
                _responseApi.StatusCode = HttpStatusCode.BadRequest;
                _responseApi.IsSuccess = false;
                _responseApi.ErrorMessages = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(_responseApi);
            }

            var producto = await _productoRepository.Update(id, updateProductoDTO);

            if (producto == null)
            {
                _responseApi.StatusCode = HttpStatusCode.BadRequest;
                _responseApi.IsSuccess = false;
                _responseApi.ErrorMessages.Add("No se pudo actualizar el producto. Puede que no exista o que la referencia esté duplicada.");

                return BadRequest(_responseApi);
            }

            _responseApi.StatusCode = HttpStatusCode.OK;
            _responseApi.IsSuccess = true;
            _responseApi.Result = producto;

            return Ok(_responseApi);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "SupervisorLogistico,Administrador")]
        public async Task<IActionResult> EliminarProducto(int id)
        {
            var resultado = await _productoRepository.Delete(id);

            if (resultado == null) {
                _responseApi.StatusCode = HttpStatusCode.NotFound;

                _responseApi.IsSuccess = false;

                _responseApi.ErrorMessages.Add("Producto no encontrado.");

                return NotFound(_responseApi);
            }

            _responseApi.StatusCode = HttpStatusCode.OK;

            _responseApi.IsSuccess = true;

            _responseApi.Result = resultado;

            return Ok(_responseApi);
        }




        [HttpPatch("{id:int}/habilitar")]
        [Authorize(Roles = "SupervisorLogistico,Administrador")]
        public async Task<IActionResult> HabilitarProducto(int id)
        {
            var actualizado = await _productoRepository.Habilitar(id);

            if (!actualizado)
            {
                _responseApi.StatusCode = HttpStatusCode.NotFound;
                _responseApi.IsSuccess = false;
                _responseApi.ErrorMessages.Add("Producto no encontrado o inactivo.");

                return NotFound(_responseApi);
            }

            _responseApi.StatusCode = HttpStatusCode.OK;
            _responseApi.IsSuccess = true;
            _responseApi.Result = "Producto habilitado correctamente.";

            return Ok(_responseApi);
        }

        [HttpPatch("{id:int}/deshabilitar")]
        [Authorize(Roles = "SupervisorLogistico,Administrador")]
        public async Task<IActionResult> DeshabilitarProducto(int id)
        {
            var actualizado = await _productoRepository.Deshabilitar(id);

            if (!actualizado)
            {
                _responseApi.StatusCode = HttpStatusCode.NotFound;
                _responseApi.IsSuccess = false;
                _responseApi.ErrorMessages.Add("Producto no encontrado.");

                return NotFound(_responseApi);
            }

            _responseApi.StatusCode = HttpStatusCode.OK;
            _responseApi.IsSuccess = true;
            _responseApi.Result = "Producto deshabilitado correctamente.";

            return Ok(_responseApi);
        }
    }
}