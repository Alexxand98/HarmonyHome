using System.Net;
using HarmonyHome.Api.Models.DTOs;
using HarmonyHome.Api.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HarmonyHome.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClienteController : ControllerBase
    {
        private readonly IClienteRepository _clienteRepository;
        private readonly ResponseApi _responseApi;

        public ClienteController(IClienteRepository clienteRepository)
        {
            _clienteRepository = clienteRepository;
            _responseApi = new ResponseApi();
        }

        [HttpGet]
        [Authorize(Roles = "Vendedor,EncargadoTienda,SupervisorLogistico,Administrador")]
        public async Task<IActionResult> GetClientes()
        {
            var clientes = await _clienteRepository.GetAll();

            _responseApi.StatusCode = HttpStatusCode.OK;
            _responseApi.Result = clientes;

            return Ok(_responseApi);
        }

        [HttpGet("activos")]
        [Authorize(Roles = "Vendedor,EncargadoTienda,SupervisorLogistico,Administrador")]
        public async Task<IActionResult> GetClientesActivos()
        {
            var clientes = await _clienteRepository.GetActivos();

            _responseApi.StatusCode = HttpStatusCode.OK;
            _responseApi.Result = clientes;

            return Ok(_responseApi);
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "Vendedor,EncargadoTienda,SupervisorLogistico,Administrador")]
        public async Task<IActionResult> GetCliente(int id)
        {
            var cliente = await _clienteRepository.GetById(id);

            if (cliente == null)
            {
                _responseApi.StatusCode = HttpStatusCode.NotFound;
                _responseApi.IsSuccess = false;
                _responseApi.ErrorMessages.Add("Cliente no encontrado.");

                return NotFound(_responseApi);
            }

            _responseApi.StatusCode = HttpStatusCode.OK;
            _responseApi.Result = cliente;

            return Ok(_responseApi);
        }

        [HttpGet("buscar")]
        [Authorize(Roles = "Vendedor,EncargadoTienda,SupervisorLogistico,Administrador")]
        public async Task<IActionResult> BuscarClientes([FromQuery] string? texto)
        {
            var clientes = await _clienteRepository.Buscar(texto);

            _responseApi.StatusCode = HttpStatusCode.OK;
            _responseApi.Result = clientes;

            return Ok(_responseApi);
        }

        [HttpPost]
        [Authorize(Roles = "Vendedor,EncargadoTienda,Administrador")]
        public async Task<IActionResult> CrearCliente([FromBody] CreateClienteDTO createClienteDTO)
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

            var cliente = await _clienteRepository.Create(createClienteDTO);

            _responseApi.StatusCode = HttpStatusCode.Created;
            _responseApi.Result = cliente;

            return StatusCode(StatusCodes.Status201Created, _responseApi);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Vendedor,EncargadoTienda,Administrador")]
        public async Task<IActionResult> ActualizarCliente(int id, [FromBody] UpdateClienteDTO updateClienteDTO)
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

            var cliente = await _clienteRepository.Update(id, updateClienteDTO);

            if (cliente == null)
            {
                _responseApi.StatusCode = HttpStatusCode.NotFound;
                _responseApi.IsSuccess = false;
                _responseApi.ErrorMessages.Add("Cliente no encontrado.");

                return NotFound(_responseApi);
            }

            _responseApi.StatusCode = HttpStatusCode.OK;
            _responseApi.Result = cliente;

            return Ok(_responseApi);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "EncargadoTienda,Administrador")]
        public async Task<IActionResult> EliminarCliente(int id)
        {
            var eliminado = await _clienteRepository.Delete(id);

            if (!eliminado)
            {
                _responseApi.StatusCode = HttpStatusCode.NotFound;
                _responseApi.IsSuccess = false;
                _responseApi.ErrorMessages.Add("Cliente no encontrado.");

                return NotFound(_responseApi);
            }

            _responseApi.StatusCode = HttpStatusCode.OK;
            _responseApi.Result = "Cliente dado de baja correctamente.";

            return Ok(_responseApi);
        }
    }
}