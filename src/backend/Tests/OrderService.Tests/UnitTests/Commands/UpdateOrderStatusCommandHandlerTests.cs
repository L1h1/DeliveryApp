using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using OrderService.Application.Commands.UpdateOrderStatus;
using OrderService.Application.Exceptions;
using OrderService.Application.Interfaces.Repositories;
using OrderService.Domain.Entities;

namespace OrderService.Tests.UnitTests.Commands
{
    public class UpdateOrderStatusCommandHandlerTests
    {
        private readonly Mock<IOrderRepository> _orderRepositoryMock;
        private readonly Mock<IDistributedCache> _distributedCacheMock;
        private readonly UpdateOrderStatusCommandHandler _updateOrderStatusCommandHandler;

        public UpdateOrderStatusCommandHandlerTests()
        {
            _orderRepositoryMock = new Mock<IOrderRepository>();
            _distributedCacheMock = new Mock<IDistributedCache>();

            _updateOrderStatusCommandHandler = new(
                _orderRepositoryMock.Object,
                _distributedCacheMock.Object);
        }

        [Fact]
        public async Task Handle_OrderExists_SuccessfulUpdate()
        {
            //Arrange
            var order = TestDataProvider.SampleOrderEntity;
            var orderId = order.Id;
            var token = CancellationToken.None;
            var status = Domain.Enums.OrderStatus.Delivered;

            _orderRepositoryMock.Setup(m => m.GetByIdAsync(orderId, token)).ReturnsAsync(order);
            _orderRepositoryMock.Setup(m => m.UpdateAsync(order, token)).Returns(Task.CompletedTask);
            _distributedCacheMock.Setup(m => m.RemoveAsync(It.IsAny<string>(), token)).Returns(Task.CompletedTask);

            //Act
            await _updateOrderStatusCommandHandler.Handle(new(orderId, status), token);

            //Assert
            order.OrderStatus.Should().Be(status);
            _orderRepositoryMock.Verify(m => m.GetByIdAsync(orderId, token), Times.Once);
            _orderRepositoryMock.Verify(m => m.UpdateAsync(order, token), Times.Once);
            _distributedCacheMock.Verify(m => m.RemoveAsync(It.IsAny<string>(), token), Times.Exactly(2));
        }

        [Fact]
        public async Task Handle_OrderDoesNotExist_ThrowsNotFoundException()
        {
            //Arrange
            var orderId = Guid.NewGuid();
            var token = CancellationToken.None;
            var status = Domain.Enums.OrderStatus.Delivered;

            _orderRepositoryMock.Setup(m => m.GetByIdAsync(orderId, token)).ReturnsAsync((Order)null);

            //Act
            Func<Task> result = () => _updateOrderStatusCommandHandler.Handle(new(orderId, status), token);

            //Assert
            result.Should().ThrowAsync<NotFoundException>();
            _orderRepositoryMock.Verify(m => m.GetByIdAsync(orderId, token), Times.Once);
            _orderRepositoryMock.Verify(m => m.UpdateAsync(null, token), Times.Never);
            _distributedCacheMock.Verify(m => m.RemoveAsync(It.IsAny<string>(), token), Times.Never);
        }
    }
}
