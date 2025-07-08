using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using OrderService.Application.Commands.AssignCourier;
using OrderService.Application.Exceptions;
using OrderService.Application.Interfaces.Repositories;
using OrderService.Application.Interfaces.Services;
using OrderService.Domain.Entities;

namespace OrderService.Tests.UnitTests.Commands
{
    public class AssignCourierCommandHandlerTests
    {
        private readonly Mock<IOrderRepository> _orderRepositoryMock;
        private readonly Mock<IUserService> _userServiceMock;
        private readonly Mock<IDistributedCache> _distributedCacheMock;
        private readonly AssignCourierCommandHandler _assignCourierCommandHandler;

        public AssignCourierCommandHandlerTests()
        {
            _orderRepositoryMock = new Mock<IOrderRepository>();
            _userServiceMock = new Mock<IUserService>();
            _distributedCacheMock = new Mock<IDistributedCache>();

            _assignCourierCommandHandler = new(
                _orderRepositoryMock.Object,
                _userServiceMock.Object,
                _distributedCacheMock.Object);
        }

        [Fact]
        public async Task Handle_ProvidedDataIsValid_SuccessfulCourierAssignment()
        {
            //Arrange
            var order = TestDataProvider.SampleOrderEntity;
            var token = CancellationToken.None;
            var courierId = Guid.NewGuid();

            _userServiceMock.Setup(m => m.GetByIdAsync(courierId.ToString(), token)).ReturnsAsync("example");
            _orderRepositoryMock.Setup(m => m.GetByIdAsync(order.Id, token)).ReturnsAsync(order);
            _orderRepositoryMock.Setup(m => m.UpdateAsync(order, token)).Returns(Task.CompletedTask);
            _distributedCacheMock.Setup(m => m.RemoveAsync(It.IsAny<string>(), token)).Returns(Task.CompletedTask);

            //Act
            await _assignCourierCommandHandler.Handle(new(order.Id, courierId), token);

            //Assert
            order.OrderStatus.Should().Be(Domain.Enums.OrderStatus.Assigned);
            order.CourierId.Should().Be(courierId);
            _userServiceMock.Verify(m => m.GetByIdAsync(courierId.ToString(), token), Times.Once);
            _orderRepositoryMock.Verify(m => m.GetByIdAsync(order.Id, token), Times.Once);
            _orderRepositoryMock.Verify(m => m.UpdateAsync(order, token), Times.Once);
            _distributedCacheMock.Verify(m => m.RemoveAsync(It.IsAny<string>(), token), Times.Exactly(2));
        }

        [Fact]
        public async Task Handle_CourierDoesNotExist_ThrowsNotFoundException()
        {
            //Arrange
            var order = TestDataProvider.SampleOrderEntity;
            var token = CancellationToken.None;
            var courierId = Guid.NewGuid();
            var errorMsg = "Courier with given id not found.";

            _userServiceMock.Setup(m => m.GetByIdAsync(courierId.ToString(), token)).ReturnsAsync(null as string);

            //Act
            var result = () => _assignCourierCommandHandler.Handle(new(order.Id, courierId), token);

            //Assert
            result.Should().ThrowAsync<NotFoundException>().WithMessage(errorMsg);
            _userServiceMock.Verify(m => m.GetByIdAsync(courierId.ToString(), token), Times.Once);
            _orderRepositoryMock.Verify(m => m.GetByIdAsync(order.Id, token), Times.Never);
            _orderRepositoryMock.Verify(m => m.UpdateAsync(order, token), Times.Never);
            _distributedCacheMock.Verify(m => m.RemoveAsync(It.IsAny<string>(), token), Times.Never);
        }

        [Fact]
        public async Task Handle_OrderDoesNotExist_ThrowsNotFoundException()
        {
            //Arrange
            var order = TestDataProvider.SampleOrderEntity;
            var token = CancellationToken.None;
            var courierId = Guid.NewGuid();
            var errorMsg = "Order with given id not found.";

            _userServiceMock.Setup(m => m.GetByIdAsync(courierId.ToString(), token)).ReturnsAsync("example");
            _orderRepositoryMock.Setup(m => m.GetByIdAsync(order.Id, token)).ReturnsAsync((Order)null);

            //Act
            var result = () => _assignCourierCommandHandler.Handle(new(order.Id, courierId), token);

            //Assert
            result.Should().ThrowAsync<NotFoundException>().WithMessage(errorMsg);
            _userServiceMock.Verify(m => m.GetByIdAsync(courierId.ToString(), token), Times.Once);
            _orderRepositoryMock.Verify(m => m.GetByIdAsync(order.Id, token), Times.Once);
            _orderRepositoryMock.Verify(m => m.UpdateAsync(order, token), Times.Never);
            _distributedCacheMock.Verify(m => m.RemoveAsync(It.IsAny<string>(), token), Times.Never);
        }
    }
}
