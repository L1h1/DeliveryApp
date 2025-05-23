using UserService.BLL.DTOs.Request;
using UserService.BLL.DTOs.Response;

namespace UserService.BLL.Interfaces
{
    public interface IAuthService
    {
        Task<UserResponseDTO> RegisterUserAsync(RegisterRequestDTO userDTO, CancellationToken cancellationToken = default);
        Task<UserResponseDTO> LoginUserAsync(LoginRequestDTO loginDTO, CancellationToken cancellationToken = default);
        Task<UserResponseDTO> UpdateTokensAsync(TokenRequestDTO tokenDTO, CancellationToken cancellationToken = default);
    }
}
