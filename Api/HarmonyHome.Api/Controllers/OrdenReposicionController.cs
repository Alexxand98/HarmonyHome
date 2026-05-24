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
    public class OrdenReposicionController : ControllerBase
    {
        private readonly IOrdenReposicionRepository _ordenReposicionRepository;

        private readonly ResponseApi _responseApi;
        public OrdenReposicionController(IOrdenReposicionRepository ordenReposicionRepository)
        {
            _ordenReposicionRepository = ordenReposicionRepository;

            _responseApi = new ResponseApi();
        }

        [HttpGet]
        [Authorize(Roles = "Logistico,SupervisorLogistico,Administrador")]
        public async Task<IActionResult> GetOrdenes()
        {
            var ordenes = await _ordenReposicionRepository.GetAll();

            _responseApi.StatusCode = HttpStatusCode.OK;

            _responseApi.Result = ordenes;

            return Ok(_responseApi);
        }

        [HttpGet("pendientes")]
        [Authorize(Roles = "Logistico,SupervisorLogistico,Administrador")]
        public async Task<IActionResult> GetPendientes()
        {
            var ordenes = await _ordenReposicionRepository.GetPendientes();

            _responseApi.StatusCode = HttpStatusCode.OK;

            _responseApi.Result = ordenes;

            return Ok(_responseApi);

        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "Logistico,SupervisorLogistico,Administrador")]
        public async Task<IActionResult> GetOrden(int id)
        {
            var orden = await _ordenReposicionRepository.GetById(id);

            if (orden == null)
            {
                _responseApi.StatusCode = HttpStatusCode.NotFound;
                _responseApi.IsSuccess = false;
                _responseApi.ErrorMessages.Add("Orden de reposición no encontrada.");

                return NotFound(_responseApi);
            }

            _responseApi.StatusCode = HttpStatusCode.OK;

            _responseApi.Result = orden;

            return Ok(_responseApi);
        }

        [HttpPost]
        [Authorize(Roles = "Vendedor,EncargadoTienda,Administrador")]
        public async Task<IActionResult> CrearOrden([FromBody] CreateOrdenReposicionDTO dto)
        {
            if (!ModelState.IsValid)
            {
                _responseApi.StatusCode = HttpStatusCode.BadRequest;
                _responseApi.IsSuccess = false;

                _responseApi.ErrorMessages = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();

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

            var orden = await _ordenReposicionRepository.Create(dto, usuarioId);

            if (orden == null)
            {
                _responseApi.StatusCode = HttpStatusCode.BadRequest;

                _responseApi.IsSuccess = false;

                _responseApi.ErrorMessages.Add("No se pudo crear la reposición. Verifica productos habilitados y stock suficiente en almacén.");

                return BadRequest(_responseApi);
            }

            _responseApi.StatusCode = HttpStatusCode.Created;

            _responseApi.Result = orden;

            return StatusCode(StatusCodes.Status201Created, _responseApi);
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

            var orden = await _ordenReposicionRepository.Asignar(id, usuarioId);

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

        [HttpPatch("{id:int}/finalizar")]
        [Authorize(Roles = "Logistico,SupervisorLogistico,Administrador")]
        public async Task<IActionResult> FinalizarOrden(int id, [FromBody] FinalizarOrdenReposicionDTO dto)
        {
            var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(usuarioId))
            {
                _responseApi.StatusCode = HttpStatusCode.Unauthorized;
                _responseApi.IsSuccess = false;

                _responseApi.ErrorMessages.Add("No se pudo identificar al usuario.");

                return Unauthorized(_responseApi);
            }

            var orden = await _ordenReposicionRepository.Finalizar(id, usuarioId, dto);

            if (orden == null)
            {
                _responseApi.StatusCode = HttpStatusCode.BadRequest;

                _responseApi.IsSuccess = false;

                _responseApi.ErrorMessages.Add("No se pudo finalizar la reposición. Verifica estado de la orden y stock suficiente.");

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
            var preparacion = await _ordenReposicionRepository.GetPreparacion(id);

            if (preparacion == null){

                _responseApi.StatusCode = HttpStatusCode.NotFound;

                _responseApi.IsSuccess = false;

                _responseApi.ErrorMessages.Add("Orden de reposición no encontrada");

                return NotFound(_responseApi);
            }

            _responseApi.StatusCode = HttpStatusCode.OK;
            _responseApi.IsSuccess = true;
            _responseApi.Result = preparacion;

            return Ok(_responseApi);
        }
    }
}