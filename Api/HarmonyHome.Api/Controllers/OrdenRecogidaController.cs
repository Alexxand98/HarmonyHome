using System.Net;
using System.Security.Claims;
using HarmonyHome.Api.Models.DTOs;
using HarmonyHome.Api.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HarmonyHome.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdenRecogidaController : ControllerBase
    {
        private readonly IOrdenRecogidaRepository _ordenRecogidaRepository;
        private readonly ResponseApi _responseApi;

        public OrdenRecogidaController(IOrdenRecogidaRepository ordenRecogidaRepository)
        {
            _ordenRecogidaRepository = ordenRecogidaRepository;
            _responseApi = new ResponseApi();
        }

        [HttpGet]
        [Authorize(Roles = "Logistico,SupervisorLogistico,Administrador")]
        public async Task<IActionResult> GetOrdenes()
        {
            var ordenes = await _ordenRecogidaRepository.GetAll();
            _responseApi.StatusCode = HttpStatusCode.OK;
            _responseApi.Result = ordenes;
            return Ok(_responseApi);
        }

        [HttpGet("pendientes")]
        [Authorize(Roles = "Logistico,SupervisorLogistico,Administrador")]
        public async Task<IActionResult> GetPendientes()
        {
            var ordenes = await _ordenRecogidaRepository.GetPendientes();
            _responseApi.StatusCode = HttpStatusCode.OK;
            _responseApi.Result = ordenes;
            return Ok(_responseApi);
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "Logistico,SupervisorLogistico,Administrador")]
        public async Task<IActionResult> GetOrden(int id)
        {
            var orden = await _ordenRecogidaRepository.GetById(id);
            if (orden == null)
            {
                _responseApi.StatusCode = HttpStatusCode.NotFound;
                _responseApi.IsSuccess = false;
                _responseApi.ErrorMessages.Add("Orden de recogida no encontrada.");
                return NotFound(_responseApi);
            }

            _responseApi.StatusCode = HttpStatusCode.OK;
            _responseApi.Result = orden;
            return Ok(_responseApi);
        }

        [HttpPatch("{id:int}/asignar")]
        [Authorize(Roles = "Logistico,SupervisorLogistico,Administrador")]
        public async Task<IActionResult> AsignarOrden(int id)
        {
            var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(usuarioId))
            {
                _responseApi.StatusCode = HttpStatusCode.Unauthorized;
                _responseApi.IsSuccess = false;
                _responseApi.ErrorMessages.Add("No se pudo identificar al usuario.");
                return Unauthorized(_responseApi);
            }

            var orden = await _ordenRecogidaRepository.Asignar(id, usuarioId);
            if (orden == null)
            {
                _responseApi.StatusCode = HttpStatusCode.BadRequest;
                _responseApi.IsSuccess = false;
                _responseApi.ErrorMessages.Add("No se pudo asignar la orden. Verifica que exista y esté pendiente.");

                return BadRequest(_responseApi);
            }
            _responseApi.StatusCode = HttpStatusCode.OK;
            _responseApi.Result = orden;

            return Ok(_responseApi);
        }
        [HttpPatch("{id:int}/preparacion")]
        [Authorize(Roles = "Logistico,SupervisorLogistico,Administrador")]
        public async Task<IActionResult> MarcarEnPreparacion(int id)
        {
            var orden = await _ordenRecogidaRepository.MarcarEnPreparacion(id);

            if (orden == null)
            {
                _responseApi.StatusCode = HttpStatusCode.BadRequest;
                _responseApi.IsSuccess = false;
                _responseApi.ErrorMessages.Add("No se pudo pasar la orden a preparación. Debe estar asignada.");
                return BadRequest(_responseApi);
            }
            _responseApi.StatusCode = HttpStatusCode.OK;
            _responseApi.Result = orden;

            return Ok(_responseApi);
        }
        [HttpPatch("{id:int}/finalizar")]
        [Authorize(Roles = "Logistico,SupervisorLogistico,Administrador")]
        public async Task<IActionResult> FinalizarOrden(int id, [FromBody] FinalizarOrdenRecogidaDTO dto)
        {
            var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(usuarioId))
            {
                _responseApi.StatusCode = HttpStatusCode.Unauthorized;
                _responseApi.IsSuccess = false;
                _responseApi.ErrorMessages.Add("No se pudo identificar al usuario.");
                return Unauthorized(_responseApi);
            }

            var orden = await _ordenRecogidaRepository.Finalizar(id, usuarioId, dto);

            if (orden == null)
            {
                _responseApi.StatusCode = HttpStatusCode.BadRequest;
                _responseApi.IsSuccess = false;
                _responseApi.ErrorMessages.Add("No se pudo finalizar la orden. Debe estar asignada o en preparación.");
                return BadRequest(_responseApi);
            }

            _responseApi.StatusCode = HttpStatusCode.OK;
            _responseApi.Result = orden;
            return Ok(_responseApi);
        }

        [HttpGet("{id:int}/preparacion")]
        [Authorize(Roles = "Logistico,SupervisorLogistico,Administrador")]
        public async Task<IActionResult> GetPreparacion(int id)
        {
            var preparacion = await _ordenRecogidaRepository.GetPreparacion(id);

            if (preparacion == null)
            {
                _responseApi.StatusCode = HttpStatusCode.NotFound;
                _responseApi.IsSuccess = false;
                _responseApi.ErrorMessages.Add("Orden de recogida no encontrada o no disponible para preparación.");

                return NotFound(_responseApi);
            }

            _responseApi.StatusCode = HttpStatusCode.OK;
            _responseApi.IsSuccess = true;
            _responseApi.Result = preparacion;

            return Ok(_responseApi);
        }
    }
}