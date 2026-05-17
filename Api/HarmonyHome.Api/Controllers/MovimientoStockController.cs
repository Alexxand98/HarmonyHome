using HarmonyHome.Api.Models.DTOs;
using HarmonyHome.Api.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace HarmonyHome.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovimientoStockController : ControllerBase
    {
        private readonly IMovimientoStockRepository _movimientoStockRepository;
        private readonly ResponseApi _responseApi;

        public MovimientoStockController(IMovimientoStockRepository movimientoStockRepository)
        {
            _movimientoStockRepository = movimientoStockRepository;

            _responseApi = new ResponseApi();
        }

        [HttpGet]
        [Authorize(Roles = "Logistico,SupervisorLogistico,Administrador")]
        public async Task<IActionResult> GetMovimientos()
        {
            var movimientos = await _movimientoStockRepository.GetAll();

            _responseApi.StatusCode = HttpStatusCode.OK;

            _responseApi.Result = movimientos;

            return Ok(_responseApi);
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "Logistico,SupervisorLogistico,Administrador")]
        public async Task<IActionResult> GetMovimiento(int id)
        {
            var movimiento = await _movimientoStockRepository.GetById(id);

            if (movimiento == null)
            {
                _responseApi.StatusCode = HttpStatusCode.NotFound;
                _responseApi.IsSuccess = false;

                _responseApi.ErrorMessages.Add("Movimiento de stock no encontrado.");

                return NotFound(_responseApi);
            }

            _responseApi.StatusCode = HttpStatusCode.OK;
            _responseApi.Result = movimiento;

            return Ok(_responseApi);
        }

        [HttpGet("producto/{productoId:int}")]
        [Authorize(Roles = "Logistico,SupervisorLogistico,Administrador")]
        public async Task<IActionResult> GetMovimientosPorProducto(int productoId)
        {

            var movimientos = await _movimientoStockRepository.GetByProducto(productoId);

            _responseApi.StatusCode = HttpStatusCode.OK;

            _responseApi.Result = movimientos;

            return Ok(_responseApi);
        }

        [HttpGet("ubicacion/{ubicacionId:int}")]
        [Authorize(Roles = "Logistico,SupervisorLogistico,Administrador")]
        public async Task<IActionResult> GetMovimientosPorUbicacion(int ubicacionId)
        {


            var movimientos = await _movimientoStockRepository.GetByUbicacion(ubicacionId);

            _responseApi.StatusCode = HttpStatusCode.OK;

            _responseApi.Result = movimientos;


            return Ok(_responseApi);
        }
    }
}