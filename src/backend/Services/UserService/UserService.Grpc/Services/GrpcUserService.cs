using Grpc.Core;
using UserService.DAL.Interfaces.Repositories;

namespace UserService.Grpc.Services
{
    public class GrpcUserService : UserService.UserServiceBase
    {
        private readonly IUserRepository _userRepository;

        public GrpcUserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public override async Task<UserReply> GetById(UserByIdRequest request, ServerCallContext context)
        {
            var user = await _userRepository.GetUserByIdAsync(request.Id, CancellationToken.None);

            if (user is null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "User with given id not found."));
            }

            return new UserReply()
            {
                Email = user.Email,
            };
        }
    }
}
