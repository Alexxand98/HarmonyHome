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
    public class PedidoClienteController : ControllerBase
    {
        private readonly IPedidoClienteRepository _pedidoClienteRepository;
        private readonly ResponseApi _responseApi;

        public PedidoClienteController(IPedidoClienteRepository pedidoClienteRepository)
        {
            _pedidoClienteRepository = pedidoClienteRepository;
            _responseApi = new ResponseApi();
        }

        [HttpPost]
        [Authorize(Roles = "Vendedor,EncargadoTienda,Administrador")]
        public async Task<IActionResult> CrearPedidoCliente([FromBody] CreatePedidoClienteDTO createPedidoClienteDTO)
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

            var pedido = await _pedidoClienteRepository.CrearPedidoCliente(createPedidoClienteDTO, usuarioId);

            if (pedido == null)
            {
                _responseApi.StatusCode = HttpStatusCode.BadRequest;
                _responseApi.IsSuccess = false;
                _responseApi.ErrorMessages.Add("No se pudo crear el pedido. Verifica cliente, productos habilitados y stock suficiente en almacén.");

                return BadRequest(_responseApi);
            }
            _responseApi.StatusCode = HttpStatusCode.Created;
            _responseApi.Result = pedido;
            return StatusCode(StatusCodes.Status201Created, _responseApi);
        }
    }
}