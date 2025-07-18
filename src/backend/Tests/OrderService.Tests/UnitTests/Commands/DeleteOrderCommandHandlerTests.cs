﻿using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Moq;
using OrderService.Application.Commands.DeleteOrder;
using OrderService.Application.Exceptions;
using OrderService.Application.Interfaces.Repositories;
using OrderService.Domain.Entities;

namespace OrderService.Tests.UnitTests.Commands
{
    public class DeleteOrderCommandHandlerTests
    {
        private readonly Mock<IOrderRepository> _orderRepositoryMock;
        private readonly Mock<IDistributedCache> _distributedCacheMock;
        private readonly Mock<ILogger<DeleteOrderCommandHandler>> _loggerMock;
        private readonly DeleteOrderCommandHandler _deleteOrderCommandHandler;

        public DeleteOrderCommandHandlerTests()
        {
            _orderRepositoryMock = new Mock<IOrderRepository>();
            _distributedCacheMock = new Mock<IDistributedCache>();
            _loggerMock = new();

            _deleteOrderCommandHandler = new(
                _orderRepositoryMock.Object,
                _loggerMock.Object,
                _distributedCacheMock.Object);
        }

        [Fact]
        public async Task Handle_OrderExists_SuccessfulDeletion()
        {
            //Arrange
            var order = TestDataProvider.SampleOrderEntity;
            var orderId = order.Id;
            var token = CancellationToken.None;

            _orderRepositoryMock.Setup(m => m.GetByIdAsync(orderId, token)).ReturnsAsync(order);
            _orderRepositoryMock.Setup(m => m.DeleteAsync(orderId, token)).Returns(Task.CompletedTask);
            _distributedCacheMock.Setup(m => m.RemoveAsync(It.IsAny<string>(), token)).Returns(Task.CompletedTask);

            //Act
            await _deleteOrderCommandHandler.Handle(new (orderId), token);

            //Assert
            _orderRepositoryMock.Verify(m => m.GetByIdAsync(orderId, token), Times.Once);
            _orderRepositoryMock.Verify(m => m.DeleteAsync(orderId, token), Times.Once);
            _distributedCacheMock.Verify(m => m.RemoveAsync(It.IsAny<string>(), token), Times.Exactly(2));
        }

        [Fact]
        public async Task Handle_OrderDoesNotExist_ThrowsNotFoundException()
        {
            //Arrange
            var orderId = Guid.NewGuid();
            var token = CancellationToken.None;

            _orderRepositoryMock.Setup(m => m.GetByIdAsync(orderId, token)).ReturnsAsync((Order)null);

            //Act
            Func<Task> result = () => _deleteOrderCommandHandler.Handle(new(orderId), token);

            //Assert
            result.Should().ThrowAsync<NotFoundException>();
            _orderRepositoryMock.Verify(m => m.GetByIdAsync(orderId, token), Times.Once);
            _orderRepositoryMock.Verify(m => m.DeleteAsync(orderId, token), Times.Never);
            _distributedCacheMock.Verify(m => m.RemoveAsync(It.IsAny<string>(), token), Times.Never);
        }
    }
}
