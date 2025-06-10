using System.Text;
using System.Text.Json;
using OrderService.Application.Interfaces.Messaging.Producers;
using RabbitMQ.Client;

namespace OrderService.Infrastructure.Messaging.Producers
{
    public class RabbitMqProducer : IMessageProducer
    {
        private readonly RabbitMqConnection _connection;

        public RabbitMqProducer(RabbitMqConnection connection)
        {
            _connection = connection;
        }

        public async Task SendMessageAsync<T>(string queue, T message)
        {
            using var channel = await _connection.Connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(
                queue: queue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            await channel.BasicPublishAsync(
                exchange: string.Empty,
                routingKey: queue,
                mandatory: true,
                basicProperties: new BasicProperties { Persistent = true },
                body: body);
        }
    }
}
