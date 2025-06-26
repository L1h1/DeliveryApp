using System.Text;
using System.Text.Json;
using DnsClient.Internal;
using Microsoft.Extensions.Logging;
using OrderService.Application.Interfaces.Messaging.Producers;
using RabbitMQ.Client;

namespace OrderService.Infrastructure.Messaging.Producers
{
    public class RabbitMqProducer : IMessageProducer
    {
        private readonly RabbitMqConnection _connection;
        private readonly ILogger<RabbitMqProducer> _logger;

        public RabbitMqProducer(RabbitMqConnection connection, ILogger<RabbitMqProducer> logger)
        {
            _connection = connection;
            _logger = logger;
        }

        public async Task SendMessageAsync<T>(string queue, T message)
        {
            _logger.LogInformation("Creating rabbitMQ channel in producer for @{queue}", queue);

            using var channel = await _connection.Connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(
                queue: queue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            _logger.LogInformation("Publishing message into rabbitMQ @{queue}", queue);

            await channel.BasicPublishAsync(
                exchange: string.Empty,
                routingKey: queue,
                mandatory: true,
                basicProperties: new BasicProperties { Persistent = true },
                body: body);

            _logger.LogInformation("Successfully published message into rabbitMQ @{queue}", queue);
        }
    }
}
