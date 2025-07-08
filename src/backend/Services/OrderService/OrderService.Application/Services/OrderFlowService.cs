using Microsoft.VisualBasic;
using OrderService.Application.DTOs.Messaging;
using OrderService.Application.Interfaces.Messaging.Producers;
using OrderService.Application.Interfaces.Services;
using OrderService.Domain.Entities;

namespace OrderService.Application.Services
{
    public class OrderFlowService : IOrderFlowService
    {
        private readonly IBackgroundJobService _backgroundJobService;
        private readonly IBillService _billService;
        private readonly IMessageProducer _messageProducer;

        public OrderFlowService(IBackgroundJobService backgroundJobService, IBillService billService, IMessageProducer messageProducer)
        {
            _backgroundJobService = backgroundJobService;
            _billService = billService;
            _messageProducer = messageProducer;
        }

        public async Task ProcessOrderCreation(Order order, string userEmail, CancellationToken cancellationToken = default)
        {
            _backgroundJobService.CreateJob(() => _messageProducer.SendMessageAsync("bills", new BillDTO
            {
                Email = userEmail,
                Contents = _billService.CreateDocumentAsync(order, cancellationToken).Result,
            }));
        }
    }
}
