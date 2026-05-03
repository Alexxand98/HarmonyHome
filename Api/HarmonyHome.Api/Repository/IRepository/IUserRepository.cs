using HarmonyHome.Api.Models.DTOs;

using HarmonyHome.Api.Models.DTOs;

namespace HarmonyHome.Api.Repository.IRepository
{
    public interface IUserRepository
    {
        bool IsUniqueUser(string email);

        Task<UserLoginResponseDTO?> Login(UserLoginDTO userLoginDTO);

        Task<UserDTO?> Register(UserRegisterDTO userRegisterDTO);

        Task<UserDTO?> GetUser(string userId);

        Task<List<UserDTO>> GetUsers();
    }
}