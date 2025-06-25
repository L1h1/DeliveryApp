using Microsoft.Extensions.Logging;
using OrderService.Application.Interfaces.Services;
using OrderService.Application.Protos;

namespace OrderService.Application.Services
{
    public class GrpcUserService : IUserService
    {
        private readonly UserService.UserServiceClient _userServiceClient;
        private readonly ILogger<GrpcUserService> _logger;

        public GrpcUserService(UserService.UserServiceClient userServiceClient, ILogger<GrpcUserService> logger)
        {
            _userServiceClient = userServiceClient;
            _logger = logger;
        }

        public async Task<string> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Requesting data via gRPC for user @{id}", id);

            var result = await _userServiceClient.GetByIdAsync(new UserByIdRequest { Id = id }, cancellationToken: cancellationToken);

            _logger.LogInformation("Successfully retrieved data via gRPC for user @{id}", id);

            return result.Email;
        }
    }
}