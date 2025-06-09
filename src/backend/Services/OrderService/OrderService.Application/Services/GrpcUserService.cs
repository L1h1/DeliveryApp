using OrderService.Application.Interfaces.Services;
using OrderService.Application.Protos;

namespace OrderService.Application.Services
{
    public class GrpcUserService : IUserService
    {
        private readonly UserService.UserServiceClient _userServiceClient;

        public GrpcUserService(UserService.UserServiceClient userServiceClient)
        {
            _userServiceClient = userServiceClient;
        }

        public async Task<string> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            var result = await _userServiceClient.GetByIdAsync(new UserByIdRequest { Id = id }, cancellationToken: cancellationToken);

            return result.Email;
        }
    }
}