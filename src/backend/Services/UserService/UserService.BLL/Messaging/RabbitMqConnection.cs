using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using UserService.DAL.Options;

namespace UserService.BLL.Messaging
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

        private void InitializeConnection(IOptions<RabbitMqOptions> rabbitMqOptions)
        {
            var factory = new ConnectionFactory
            {
                HostName = rabbitMqOptions.Value.Host,
                UserName = rabbitMqOptions.Value.Username,
                Password = rabbitMqOptions.Value.Password,
            };

            Connection = factory.CreateConnectionAsync().Result;
        }
    }
}
