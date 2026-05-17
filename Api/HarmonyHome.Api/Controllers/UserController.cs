using HarmonyHome.Api.Models.DTOs;
using HarmonyHome.Api.Models.DTOs.UsuarioDto;
using HarmonyHome.Api.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace HarmonyHome.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "SupervisorLogistico,Administrador")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        private readonly ResponseApi _responseApi;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;

            _responseApi = new ResponseApi();
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userRepository.GetUsers();

            _responseApi.StatusCode = HttpStatusCode.OK;

            _responseApi.Result = users;

            return Ok(_responseApi);
        }

        [HttpGet("logisticos")]
        public async Task<IActionResult> GetLogisticos()
        {
            var users = await _userRepository.GetLogisticos();

            _responseApi.StatusCode = HttpStatusCode.OK;

            _responseApi.Result = users;

            return Ok(_responseApi);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(string id)
        {
            var user = await _userRepository.GetUser(id);

            if (user == null)
            {
                _responseApi.StatusCode = HttpStatusCode.NotFound;
                _responseApi.IsSuccess = false;

                _responseApi.ErrorMessages.Add("Usuario no encontrado.");

                return NotFound(_responseApi);
            }

            _responseApi.StatusCode = HttpStatusCode.OK;
            _responseApi.Result = user;

            return Ok(_responseApi);
        }

        [HttpPost("logistico")]
        public async Task<IActionResult> CreateLogistico([FromBody] CreateLogisticoDTO createLogisticoDTO)
        {
            if (!ModelState.IsValid)
            {
                _responseApi.StatusCode = HttpStatusCode.BadRequest;
                _responseApi.IsSuccess = false;

                _responseApi.ErrorMessages = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();

                return BadRequest(_responseApi);
            }

            var user = await _userRepository.CreateLogistico(createLogisticoDTO);

            if (user == null)
            {
                _responseApi.StatusCode = HttpStatusCode.BadRequest;
                _responseApi.IsSuccess = false;

                _responseApi.ErrorMessages.Add("No se pudo crear el logistico");

                return BadRequest(_responseApi);
            }

            _responseApi.StatusCode = HttpStatusCode.Created;
            _responseApi.Result = user;

            return StatusCode(StatusCodes.Status201Created, _responseApi);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserDTO updateUserDTO)
        {
            if (!ModelState.IsValid)
            {
                _responseApi.StatusCode = HttpStatusCode.BadRequest;
                _responseApi.IsSuccess = false;
                _responseApi.ErrorMessages = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();

                return BadRequest(_responseApi);
            }

            var user = await _userRepository.UpdateUser(id, updateUserDTO);

            if (user == null)
            {
                _responseApi.StatusCode = HttpStatusCode.BadRequest;
                _responseApi.IsSuccess = false;
                _responseApi.ErrorMessages.Add("No se pudo actualizar el usuario");

                return BadRequest(_responseApi);
            }

            _responseApi.StatusCode = HttpStatusCode.OK;

            _responseApi.Result = user;

            return Ok(_responseApi);
        }

        [HttpPatch("{id}/activar")]
        public async Task<IActionResult> ActivarUser(string id)
        {
            var actualizado = await _userRepository.ActivarUser(id);

            if (!actualizado)
            {
                _responseApi.StatusCode = HttpStatusCode.NotFound;
                _responseApi.IsSuccess = false;
                _responseApi.ErrorMessages.Add("Usuario no encontrado.");

                return NotFound(_responseApi);
            }

            _responseApi.StatusCode = HttpStatusCode.OK;

            _responseApi.Result = "Usuario activado";

            return Ok(_responseApi);
        }

        [HttpPatch("{id}/desactivar")]
        public async Task<IActionResult> DesactivarUser(string id)
        {
            var actualizado = await _userRepository.DesactivarUser(id);

            if (!actualizado){
                _responseApi.StatusCode = HttpStatusCode.NotFound;
                _responseApi.IsSuccess = false;
                _responseApi.ErrorMessages.Add("Usuario no encontrado");

                return NotFound(_responseApi);
            }

            _responseApi.StatusCode = HttpStatusCode.OK;

            _responseApi.Result = "Usuario desactivado";

            return Ok(_responseApi);
        }
    }
}