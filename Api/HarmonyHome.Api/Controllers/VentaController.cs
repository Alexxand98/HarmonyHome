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
    public class VentaController : ControllerBase
    {
        private readonly IVentaRepository _ventaRepository;
        private readonly ResponseApi _responseApi;

        public VentaController(IVentaRepository ventaRepository)
        {
            _ventaRepository = ventaRepository;
            _responseApi = new ResponseApi();
        }

        [HttpPost("directa")]
        [Authorize(Roles = "Vendedor,EncargadoTienda,Administrador")]
        public async Task<IActionResult> CrearVentaDirecta([FromBody] CreateVentaDirectaDTO createVentaDirectaDTO)
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

            var venta = await _ventaRepository.CrearVentaDirecta(createVentaDirectaDTO, usuarioId);

            if (venta == null)
            {
                _responseApi.StatusCode = HttpStatusCode.BadRequest;
                _responseApi.IsSuccess = false;
                _responseApi.ErrorMessages.Add("No se pudo realizar la venta. Verifica cliente, productos habilitados y stock suficiente en tienda.");

                return BadRequest(_responseApi);
            }

            _responseApi.StatusCode = HttpStatusCode.Created;
            _responseApi.Result = venta;

            return StatusCode(StatusCodes.Status201Created, _responseApi);
        }
    }
}