using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OrderService.Application.DTOs.Response;
using OrderService.Application.Exceptions;
using OrderService.Application.Interfaces.Repositories;
using OrderService.Application.Queries.GetOrdersByStatus;
using OrderService.Domain.Entities;
using System.Linq.Expressions;

namespace OrderService.Tests.UnitTests.Queries
{
    public class GetOrdersByStatusQueryHandlerTests
    {
        private readonly IMapper _mapper;
        private readonly Mock<IOrderRepository> _orderRepositoryMock;
        private readonly GetOrdersByStatusQueryHandler _handler;
        private readonly Mock<ILogger<GetOrdersByStatusQueryHandler>> _loggerMock;

        public GetOrdersByStatusQueryHandlerTests()
        {
            _orderRepositoryMock = new();
            _loggerMock = new();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddMaps(typeof(GetOrdersByStatusQueryHandler).Assembly);
            });
            _mapper = new Mapper(config);

            _handler = new(_mapper, _orderRepositoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_OrdersWithStatusDoNotExist_ThrowsNotFoundException()
        {
            //Arrange
            var errorMsg = "No orders with given status found.";
            var token = CancellationToken.None;
            var status = Domain.Enums.OrderStatus.Created;

            _orderRepositoryMock.Setup(m => m.ListAsync(It.IsAny<Expression<Func<Order, bool>>>(), token)).ReturnsAsync(new List<Order>());

            //Act
            var result = () => _handler.Handle(new(status), token);

            //Assert
            result.Should().ThrowAsync<NotFoundException>().WithMessage(errorMsg);
            _orderRepositoryMock.Verify(m => m.ListAsync(It.IsAny<Expression<Func<Order, bool>>>(), token), Times.Once);
        }

        [Fact]
        public async Task Handle_OrdersWithStatusExist_ReturnsData()
        {
            //Arrange
            var orders = Enumerable.Range(1, 10).Select(i => TestDataProvider.SampleOrderEntity).ToList();
            var mappedOrders = _mapper.Map<List<OrderResponseDTO>>(orders);
            var errorMsg = "No orders with given status found.";
            var token = CancellationToken.None;
            var status = Domain.Enums.OrderStatus.Created;

            _orderRepositoryMock.Setup(m => m.ListAsync(It.IsAny<Expression<Func<Order, bool>>>(), token)).ReturnsAsync(orders);

            //Act
            var result = await _handler.Handle(new(status), token);

            //Assert
            result.Should().BeEquivalentTo(mappedOrders);
            _orderRepositoryMock.Verify(m => m.ListAsync(It.IsAny<Expression<Func<Order, bool>>>(), token), Times.Once);
        }
    }
}
