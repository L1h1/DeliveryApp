using Microsoft.Extensions.Options;
using OrderService.Application.Options;
using RabbitMQ.Client;

namespace OrderService.Infrastructure.Messaging
{
    public class RabbitMqConnection : IDisposable
    {
        public IConnection Connection { get; private set; }

        public RabbitMqConnection(IOptions<RabbitMqOptions> rabbitMqOptions)
        {
            InitializeConnection(rabbitMqOptions);
        }

        public void Dispose()
        {
            Connection?.Dispose();
        }

        private async Task InitializeConnection(IOptions<RabbitMqOptions> rabbitMqOptions)
        {
            var factory = new ConnectionFactory
            {
                HostName = rabbitMqOptions.Value.Host,
                UserName = rabbitMqOptions.Value.Username,
                Password = rabbitMqOptions.Value.Password,
            };

            Connection = await factory.CreateConnectionAsync();
        }
    }
}
