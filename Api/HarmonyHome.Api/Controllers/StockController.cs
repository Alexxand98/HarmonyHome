using HarmonyHome.Api.Models.DTOs;
using HarmonyHome.Api.Models.DTOs.StockDto;
using HarmonyHome.Api.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace HarmonyHome.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockController : ControllerBase
    {
        private readonly IStockRepository _stockRepository;
        private readonly ResponseApi _responseApi;

        public StockController(IStockRepository stockRepository)
        {
            _stockRepository = stockRepository;
            _responseApi = new ResponseApi();
        }

        [HttpGet]
        [Authorize(Roles = "Logistico,SupervisorLogistico,Administrador")]
        public async Task<IActionResult> GetStock()
        {
            var stock = await _stockRepository.GetAll();

            _responseApi.StatusCode = HttpStatusCode.OK;
            _responseApi.Result = stock;

            return Ok(_responseApi);
        }

        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<IActionResult> GetStockById(int id)
        {
            var stock = await _stockRepository.GetById(id);

            if (stock == null)
            {
                _responseApi.StatusCode = HttpStatusCode.NotFound;
                _responseApi.IsSuccess = false;
                _responseApi.ErrorMessages.Add("Registro de stock no encontrado.");

                return NotFound(_responseApi);
            }

            _responseApi.StatusCode = HttpStatusCode.OK;
            _responseApi.Result = stock;

            return Ok(_responseApi);
        }

        [HttpGet("producto/{productoId:int}")]
        [Authorize]
        public async Task<IActionResult> GetStockByProducto(int productoId)
        {
            var stock = await _stockRepository.GetByProducto(productoId);

            _responseApi.StatusCode = HttpStatusCode.OK;
            _responseApi.Result = stock;

            return Ok(_responseApi);
        }

        [HttpGet("ubicacion/{ubicacionId:int}")]
        [Authorize(Roles = "Logistico,SupervisorLogistico,Administrador")]
        public async Task<IActionResult> GetStockByUbicacion(int ubicacionId)
        {
            var stock = await _stockRepository.GetByUbicacion(ubicacionId);

            _responseApi.StatusCode = HttpStatusCode.OK;
            _responseApi.Result = stock;

            return Ok(_responseApi);
        }

        [HttpGet("tienda")]
        [Authorize]
        public async Task<IActionResult> GetStockTienda()
        {
            var stock = await _stockRepository.GetStockTienda();

            _responseApi.StatusCode = HttpStatusCode.OK;
            _responseApi.Result = stock;

            return Ok(_responseApi);
        }

        [HttpGet("almacen")]
        [Authorize]
        public async Task<IActionResult> GetStockAlmacen()
        {
            var stock = await _stockRepository.GetStockAlmacen();

            _responseApi.StatusCode = HttpStatusCode.OK;
            _responseApi.Result = stock;

            return Ok(_responseApi);
        }

        [HttpGet("resumen/producto/{productoId:int}")]
        [Authorize]
        public async Task<IActionResult> GetResumenProducto(int productoId)
        {
            var resumen = await _stockRepository.GetResumenByProducto(productoId);

            if (resumen == null)
            {
                _responseApi.StatusCode = HttpStatusCode.NotFound;
                _responseApi.IsSuccess = false;
                _responseApi.ErrorMessages.Add("Producto no encontrado.");

                return NotFound(_responseApi);
            }

            _responseApi.StatusCode = HttpStatusCode.OK;
            _responseApi.Result = resumen;

            return Ok(_responseApi);
        }

        [HttpPost]
        [Authorize(Roles = "SupervisorLogistico,Administrador")]
        public async Task<IActionResult> CrearStock([FromBody] CreateStockUbicacionDTO createStockDTO)
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

            var stock = await _stockRepository.Create(createStockDTO);

            if (stock == null)
            {
                _responseApi.StatusCode = HttpStatusCode.BadRequest;
                _responseApi.IsSuccess = false;
                _responseApi.ErrorMessages.Add("No se pudo crear el stock. Verifica producto y ubicación.");

                return BadRequest(_responseApi);
            }

            _responseApi.StatusCode = HttpStatusCode.Created;
            _responseApi.Result = stock;

            return StatusCode(StatusCodes.Status201Created, _responseApi);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "SupervisorLogistico,Administrador")]
        public async Task<IActionResult> ActualizarStock(int id, [FromBody] UpdateStockUbicacionDTO updateStockDTO)
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

            var stock = await _stockRepository.Update(id, updateStockDTO);

            if (stock == null)
            {
                _responseApi.StatusCode = HttpStatusCode.NotFound;
                _responseApi.IsSuccess = false;
                _responseApi.ErrorMessages.Add("Registro de stock no encontrado.");

                return NotFound(_responseApi);
            }

            _responseApi.StatusCode = HttpStatusCode.OK;
            _responseApi.Result = stock;

            return Ok(_responseApi);
        }




        [HttpPost("mover")]
        [Authorize(Roles = "Logistico,SupervisorLogistico,Administrador")]
        public async Task<IActionResult> MoverStock([FromBody] MoveStockDTO moveStockDTO)
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

            var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(usuarioId))
            {
                _responseApi.StatusCode = HttpStatusCode.Unauthorized;
                _responseApi.IsSuccess = false;
                _responseApi.ErrorMessages.Add("No se pudo identificar al usuario.");

                return Unauthorized(_responseApi);
            }

            var movimiento = await _stockRepository.MoverStock(moveStockDTO, usuarioId);

            if (movimiento == null)
            {
                _responseApi.StatusCode = HttpStatusCode.BadRequest;
                _responseApi.IsSuccess = false;
                _responseApi.ErrorMessages.Add("No se pudo mover el stock. Verifica producto, ubicaciones internas, stock suficiente y que no participe la tienda.");

                return BadRequest(_responseApi);
            }

            _responseApi.StatusCode = HttpStatusCode.Created;
            _responseApi.Result = movimiento;

            return StatusCode(StatusCodes.Status201Created, _responseApi);
        }

        [HttpGet("bajo-minimo/tienda")]
        [Authorize(Roles = "Vendedor,EncargadoTienda,Administrador")]
        public async Task<IActionResult> GetProductosBajoStockTienda()
        {
            var productos = await _stockRepository.GetProductosBajoStockTienda();

            _responseApi.StatusCode = HttpStatusCode.OK;
            _responseApi.IsSuccess = true;
            _responseApi.Result = productos;

            return Ok(_responseApi);
        }

        [HttpGet("bajo-minimo/general")]
        [Authorize(Roles = "Logistico,SupervisorLogistico,Administrador")]
        public async Task<IActionResult> GetProductosBajoStockGeneral()
        {
            var productos = await _stockRepository.GetProductosBajoStockGeneral();

            _responseApi.StatusCode = HttpStatusCode.OK;
            _responseApi.IsSuccess = true;
            _responseApi.Result = productos;

            return Ok(_responseApi);
        }
    }
}