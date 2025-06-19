namespace OrderService.Application.Interfaces.Messaging.Producers
{
    public interface IMessageProducer
    {
        public Task SendMessageAsync<T>(string queue, T message);
    }
}
