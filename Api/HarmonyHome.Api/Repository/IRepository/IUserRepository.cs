using HarmonyHome.Api.Models.DTOs;

using HarmonyHome.Api.Models.DTOs;
using HarmonyHome.Api.Models.DTOs.UsuarioDto;

namespace HarmonyHome.Api.Repository.IRepository
{
    public interface IUserRepository
    {
        bool IsUniqueUser(string email);

        Task<UserLoginResponseDTO?> Login(UserLoginDTO userLoginDTO);

        Task<UserDTO?> Register(UserRegisterDTO userRegisterDTO);

        Task<UserDTO?> GetUser(string userId);

        Task<List<UserDTO>> GetUsers();


        Task<List<UserDTO>> GetLogisticos();

        Task<UserDTO?> CreateLogistico(CreateLogisticoDTO createLogisticoDTO);

        Task<UserDTO?> UpdateUser(string id, UpdateUserDTO updateUserDTO);

        Task<bool> ActivarUser(string id);

        Task<bool> DesactivarUser(string id);
    }
}