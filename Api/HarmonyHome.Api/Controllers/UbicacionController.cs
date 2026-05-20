using HarmonyHome.Api.Models.DTO;
using HarmonyHome.Api.Models.DTOs;
using HarmonyHome.Api.Models.Enums;
using HarmonyHome.Api.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace HarmonyHome.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UbicacionController : ControllerBase
    {
        private readonly IUbicacionRepository _ubicacionRepository;
        private readonly ResponseApi _responseApi;

        public UbicacionController(IUbicacionRepository ubicacionRepository)
        {
            _ubicacionRepository = ubicacionRepository;
            _responseApi = new ResponseApi();
        }

        [HttpGet]
        [Authorize(Roles = "Logistico,SupervisorLogistico,Administrador")]
        public async Task<IActionResult> GetUbicaciones()
        {
            var ubicaciones = await _ubicacionRepository.GetAll();

            _responseApi.StatusCode = HttpStatusCode.OK;
            _responseApi.Result = ubicaciones;

            return Ok(_responseApi);
        }

        [HttpGet("activas")]
        [Authorize]
        public async Task<IActionResult> GetUbicacionesActivas()
        {
            var ubicaciones = await _ubicacionRepository.GetActivas();

            _responseApi.StatusCode = HttpStatusCode.OK;
            _responseApi.Result = ubicaciones;

            return Ok(_responseApi);
        }

        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<IActionResult> GetUbicacion(int id)
        {
            var ubicacion = await _ubicacionRepository.GetById(id);

            if (ubicacion == null)
            {
                _responseApi.StatusCode = HttpStatusCode.NotFound;
                _responseApi.IsSuccess = false;
                _responseApi.ErrorMessages.Add("Ubicación no encontrada.");
                return NotFound(_responseApi);
            }

            _responseApi.StatusCode = HttpStatusCode.OK;
            _responseApi.Result = ubicacion;

            return Ok(_responseApi);
        }

        [HttpGet("codigo/{codigo}")]
        [Authorize]
        public async Task<IActionResult> GetUbicacionPorCodigo(string codigo)
        {
            var ubicacion = await _ubicacionRepository.GetByCodigo(codigo);

            if (ubicacion == null)
            {
                _responseApi.StatusCode = HttpStatusCode.NotFound;
                _responseApi.IsSuccess = false;
                _responseApi.ErrorMessages.Add("Ubicación no encontrada.");
                return NotFound(_responseApi);
            }

            _responseApi.StatusCode = HttpStatusCode.OK;
            _responseApi.Result = ubicacion;

            return Ok(_responseApi);
        }

        [HttpGet("tipo/{tipoUbicacion}")]
        [Authorize]
        public async Task<IActionResult> GetUbicacionesPorTipo(TipoUbicacion tipoUbicacion)
        {
            var ubicaciones = await _ubicacionRepository.GetByTipo(tipoUbicacion);

            _responseApi.StatusCode = HttpStatusCode.OK;
            _responseApi.Result = ubicaciones;

            return Ok(_responseApi);
        }

        [HttpPost]
        [Authorize(Roles = "SupervisorLogistico,Administrador")]
        public async Task<IActionResult> CrearUbicacion([FromBody] CreateUbicacionDTO createUbicacionDTO)
        {
            if (!ModelState.IsValid)
            {
                _responseApi.StatusCode = HttpStatusCode.BadRequest;
                _responseApi.IsSuccess = false;
                _responseApi.ErrorMessages = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(_responseApi);
            }

            var ubicacion = await _ubicacionRepository.Create(createUbicacionDTO);

            if (ubicacion == null)
            {
                _responseApi.StatusCode = HttpStatusCode.BadRequest;
                _responseApi.IsSuccess = false;
                _responseApi.ErrorMessages.Add("Ya existe una ubicación con ese código.");
                return BadRequest(_responseApi);
            }

            _responseApi.StatusCode = HttpStatusCode.Created;
            _responseApi.Result = ubicacion;

            return StatusCode(StatusCodes.Status201Created, _responseApi);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "SupervisorLogistico,Administrador")]
        public async Task<IActionResult> ActualizarUbicacion(int id, [FromBody] UpdateUbicacionDTO updateUbicacionDTO)
        {
            if (!ModelState.IsValid)
            {
                _responseApi.StatusCode = HttpStatusCode.BadRequest;
                _responseApi.IsSuccess = false;
                _responseApi.ErrorMessages = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(_responseApi);
            }

            var ubicacion = await _ubicacionRepository.Update(id, updateUbicacionDTO);

            if (ubicacion == null)
            {
                _responseApi.StatusCode = HttpStatusCode.BadRequest;
                _responseApi.IsSuccess = false;
                _responseApi.ErrorMessages.Add("No se pudo actualizar la ubicación. Puede que no exista o que el código esté duplicado.");
                return BadRequest(_responseApi);
            }

            _responseApi.StatusCode = HttpStatusCode.OK;
            _responseApi.Result = ubicacion;

            return Ok(_responseApi);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "SupervisorLogistico,Administrador")]
        public async Task<IActionResult> EliminarUbicacion(int id)
        {
            var resultado = await _ubicacionRepository.Delete(id);

            if (resultado == null){
                _responseApi.StatusCode = HttpStatusCode.NotFound;
                _responseApi.IsSuccess = false;
                _responseApi.ErrorMessages.Add("Ubicacion no encontrada.");

                return NotFound(_responseApi);
            }

            _responseApi.StatusCode = HttpStatusCode.OK;
            _responseApi.IsSuccess = true;
            _responseApi.Result = resultado;

            return Ok(_responseApi);
        }
    }
}