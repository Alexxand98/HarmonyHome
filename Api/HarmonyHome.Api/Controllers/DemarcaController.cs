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
    public class DemarcaController : ControllerBase
    {
        private readonly IDemarcaRepository _demarcaRepository;

        private readonly ResponseApi _responseApi;

        public DemarcaController(IDemarcaRepository demarcaRepository)
        {
            _demarcaRepository = demarcaRepository;

            _responseApi = new ResponseApi();
        }

        [HttpPost]
        [Authorize(Roles = "Vendedor,Logistico,SupervisorLogistico,Administrador")]
        public async Task<IActionResult> CrearDemarca([FromBody] CreateDemarcaDTO dto)
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

            var rolUsuario = User.FindFirstValue(ClaimTypes.Role);

            if (string.IsNullOrEmpty(usuarioId) || string.IsNullOrEmpty(rolUsuario))
            {
                _responseApi.StatusCode = HttpStatusCode.Unauthorized;
                _responseApi.IsSuccess = false;

                _responseApi.ErrorMessages.Add("No se pudo identificar al usuario o su rol.");

                return Unauthorized(_responseApi);
            }

            var demarca = await _demarcaRepository.CrearDemarca(dto, usuarioId, rolUsuario);

            if (demarca == null)
            {
                _responseApi.StatusCode = HttpStatusCode.BadRequest;

                _responseApi.IsSuccess = false;

                _responseApi.ErrorMessages.Add("No se pudo realizar la demarca. Verifica producto, ubicación, stock suficiente y permisos por rol.");

                return BadRequest(_responseApi);
            }

            _responseApi.StatusCode = HttpStatusCode.Created;


            _responseApi.Result = demarca;

            return StatusCode(StatusCodes.Status201Created, _responseApi);
        }
    }
}