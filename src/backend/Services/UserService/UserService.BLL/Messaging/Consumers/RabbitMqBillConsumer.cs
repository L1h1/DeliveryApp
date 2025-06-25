using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using UserService.BLL.Constants;
using UserService.BLL.DTOs.Messaging;
using UserService.BLL.Interfaces;

namespace UserService.BLL.Messaging.Consumers
{
    public class RabbitMqBillConsumer : BackgroundService
    {
        private readonly RabbitMqConnection _connection;
        private readonly IServiceScopeFactory _scopeFactory;
        private IChannel _channel;
        private readonly ILogger<RabbitMqBillConsumer> _logger;

        public RabbitMqBillConsumer(
            RabbitMqConnection connection,
            IServiceScopeFactory scopeFactory,
            ILogger<RabbitMqBillConsumer> logger)
        {
            _connection = connection;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            _channel = await _connection.Connection.CreateChannelAsync();

            var queueName = "bills";

            await _channel.QueueDeclareAsync(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (sender, eventArgs) =>
            {
                using var scope = _scopeFactory.CreateScope();
                var jobService = scope.ServiceProvider.GetRequiredService<IBackgroundJobService>();
                var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                var body = eventArgs.Body.ToArray();
                var jsonMessage = Encoding.UTF8.GetString(body);
                var message = JsonSerializer.Deserialize<BillDTO>(jsonMessage);

                _logger.LogInformation("Recieved bill message for @{email} through rabbitMQ broker", message.Email);

                jobService.CreateJob(() =>
                    emailService.SendEmailAsync(message.Email, EmailConstants.OrderCreatedBill, $"<pre>{message.Contents}</pre>"));

                await ((AsyncEventingBasicConsumer)sender).Channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false);
            };

            await _channel.BasicConsumeAsync(
                queue: queueName,
                autoAck: false,
                consumer: consumer);
        }
    }
}
