namespace OrderService.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<string> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    }
}
