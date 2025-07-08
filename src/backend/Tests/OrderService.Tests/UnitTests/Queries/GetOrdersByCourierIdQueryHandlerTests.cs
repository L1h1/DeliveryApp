using AutoMapper;
using FluentAssertions;
using Moq;
using OrderService.Application.DTOs.Response;
using OrderService.Application.Exceptions;
using OrderService.Application.Interfaces.Repositories;
using OrderService.Application.Interfaces.Services;
using OrderService.Application.Queries.GetOrdersByCourierId;
using OrderService.Domain.Entities;
using System.Linq.Expressions;

namespace OrderService.Tests.UnitTests.Queries
{
    public class GetOrdersByCourierIdQueryHandlerTests
    {
        private readonly Mock<IUserService> _userServiceMock;
        private readonly Mock<IOrderRepository> _orderRepositoryMock;
        private IMapper _mapper;
        private readonly GetOrdersByCourierIdQueryHandler _handler;

        public GetOrdersByCourierIdQueryHandlerTests()
        {
            _userServiceMock = new();
            _orderRepositoryMock = new();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddMaps(typeof(GetOrdersByCourierIdQueryHandler).Assembly);
            });
            _mapper = new Mapper(config);

            _handler = new(
                _mapper,
                _userServiceMock.Object,
                _orderRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_CourierDoesNotExist_ThrowsNotFoundException()
        {
            //Arrange
            var courierId = Guid.NewGuid();
            var token = CancellationToken.None;
            var errorMsg = "User with given id not found.";

            _userServiceMock.Setup(m => m.GetByIdAsync(courierId.ToString(), token)).ReturnsAsync(null as string);

            //Act
            var result = () => _handler.Handle(new(courierId), token);

            //Assert
            result.Should().ThrowAsync<NotFoundException>().WithMessage(errorMsg);
            _userServiceMock.Verify(m => m.GetByIdAsync(courierId.ToString(), token), Times.Once);
            _orderRepositoryMock.Verify(m => m.ListAsync(It.IsAny<Expression<Func<Order, bool>>>(), token), Times.Never);
        }

        [Fact]
        public async Task Handle_OrdersDoNotExist_ThrowsNotFoundException()
        {
            //Arrange
            var courierId = Guid.NewGuid();
            var token = CancellationToken.None;
            var errorMsg = "No orders found.";

            _userServiceMock.Setup(m => m.GetByIdAsync(courierId.ToString(), token)).ReturnsAsync(courierId.ToString());
            _orderRepositoryMock.Setup(m => m.ListAsync(It.IsAny<Expression<Func<Order, bool>>>(), token)).ReturnsAsync(new List<Order>());

            //Act
            var result = () => _handler.Handle(new(courierId), token);

            //Assert
            result.Should().ThrowAsync<NotFoundException>().WithMessage(errorMsg);
            _userServiceMock.Verify(m => m.GetByIdAsync(courierId.ToString(), token), Times.Once);
            _orderRepositoryMock.Verify(m => m.ListAsync(It.IsAny<Expression<Func<Order, bool>>>(), token), Times.Once);
        }

        [Fact]
        public async Task Handle_CourierAndOrdersExist_ReturnsData()
        {
            //Arrange
            var courierId = Guid.NewGuid();
            var token = CancellationToken.None;
            var orders = Enumerable.Range(1, 10).Select(i => TestDataProvider.SampleOrderEntity).ToList();
            var ordersDTO = _mapper.Map<List<OrderResponseDTO>>(orders);

            _userServiceMock.Setup(m => m.GetByIdAsync(courierId.ToString(), token)).ReturnsAsync(courierId.ToString());
            _orderRepositoryMock.Setup(m => m.ListAsync(It.IsAny<Expression<Func<Order, bool>>>(), token)).ReturnsAsync(orders);

            //Act
            var result = await _handler.Handle(new(courierId), token);

            //Assert
            result.Should().BeEquivalentTo(ordersDTO);
            _userServiceMock.Verify(m => m.GetByIdAsync(courierId.ToString(), token), Times.Once);
            _orderRepositoryMock.Verify(m => m.ListAsync(It.IsAny<Expression<Func<Order, bool>>>(), token), Times.Once);
        }
    }
}
