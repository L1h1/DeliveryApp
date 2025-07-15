using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Moq;
using OrderService.Application.DTOs.Response;
using OrderService.Application.Exceptions;
using OrderService.Application.Interfaces.Repositories;
using OrderService.Application.Interfaces.Services;
using OrderService.Application.Queries.GetOrdersByClientId;
using OrderService.Domain.Entities;
using System.Linq.Expressions;
using System.Text;
using System.Text.Json;

namespace OrderService.Tests.UnitTests.Queries
{
    public class GetOrdersByClientIdQueryHandlerTests
    {
        private readonly Mock<IUserService> _userServiceMock;
        private readonly Mock<IOrderRepository> _orderRepositoryMock;
        private readonly Mock<IDistributedCache> _distributedCacheMock;
        private IMapper _mapper;
        private readonly Mock<ILogger<GetOrdersByClientIdQueryHandler>> _loggerMock;
        private readonly GetOrdersByClientIdQueryHandler _handler;

        public GetOrdersByClientIdQueryHandlerTests()
        {
            _userServiceMock = new();
            _orderRepositoryMock = new();
            _loggerMock = new();
            _distributedCacheMock = new();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddMaps(typeof(GetOrdersByClientIdQueryHandler).Assembly);
            });
            _mapper = new Mapper(config);

            _handler = new(
                _mapper,
                _userServiceMock.Object,
                _orderRepositoryMock.Object,
                _loggerMock.Object,
                _distributedCacheMock.Object);
        }

        [Fact]
        public async Task Handle_OrdersExistInCache_ReturnsData()
        {
            //Arrange
            var orders = Enumerable.Range(1, 10).Select(i => TestDataProvider.SampleOrderEntity).ToList();
            var ordersDTO = _mapper.Map<List<OrderResponseDTO>>(orders);
            var serializedDTO = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(ordersDTO));
            var clientId = Guid.NewGuid();
            var token = CancellationToken.None;

            _distributedCacheMock.Setup(m => m.GetAsync(It.IsAny<string>(), token)).ReturnsAsync(serializedDTO);

            //Act
            var result = await _handler.Handle(new(clientId), token);

            //Assert
            result.Should().BeEquivalentTo(ordersDTO);
            _distributedCacheMock.Verify(m => m.GetAsync(It.IsAny<string>(), token), Times.Once);
            _userServiceMock.Verify(m => m.GetByIdAsync(clientId.ToString(), token), Times.Never);
            _orderRepositoryMock.Verify(m => m.ListAsync(It.IsAny<Expression<Func<Order, bool>>>(), token), Times.Never);
            _distributedCacheMock.Verify(m => m.SetAsync(
                It.IsAny<string>(),
                It.IsAny<byte[]>(),
                It.IsAny<DistributedCacheEntryOptions>(), token),
                Times.Never);
        }

        [Fact]
        public async Task Handle_UserDoesNotExist_ThrowNotFoundException()
        {
            //Arrange
            var orders = Enumerable.Range(1, 10).Select(i => TestDataProvider.SampleOrderEntity).ToList();
            var ordersDTO = _mapper.Map<List<OrderResponseDTO>>(orders);
            var serializedDTO = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(ordersDTO));
            var clientId = Guid.NewGuid();
            var token = CancellationToken.None;
            var errorMsg = "User with given id not found.";

            _distributedCacheMock.Setup(m => m.GetAsync(It.IsAny<string>(), token)).ReturnsAsync(null as byte[]);
            _userServiceMock.Setup(m => m.GetByIdAsync(clientId.ToString(), token)).ReturnsAsync(null as string);

            //Act
            var result = () => _handler.Handle(new(clientId), token);

            //Assert
            result.Should().ThrowAsync<NotFoundException>().WithMessage(errorMsg);
            _distributedCacheMock.Verify(m => m.GetAsync(It.IsAny<string>(), token), Times.Once);
            _userServiceMock.Verify(m => m.GetByIdAsync(clientId.ToString(), token), Times.Once);
            _orderRepositoryMock.Verify(m => m.ListAsync(It.IsAny<Expression<Func<Order, bool>>>(), token), Times.Never);
            _distributedCacheMock.Verify(m => m.SetAsync(
                It.IsAny<string>(),
                It.IsAny<byte[]>(),
                It.IsAny<DistributedCacheEntryOptions>(), token),
                Times.Never);
        }

        [Fact]
        public async Task Handle_OrdersDoNotExistInDb_ThrowNotFoundException()
        {
            //Arrange
            var orders = Enumerable.Range(1, 10).Select(i => TestDataProvider.SampleOrderEntity).ToList();
            var ordersDTO = _mapper.Map<List<OrderResponseDTO>>(orders);
            var serializedDTO = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(ordersDTO));
            var clientId = Guid.NewGuid();
            var token = CancellationToken.None;
            var errorMsg = "No orders found.";

            _distributedCacheMock.Setup(m => m.GetAsync(It.IsAny<string>(), token)).ReturnsAsync(null as byte[]);
            _userServiceMock.Setup(m => m.GetByIdAsync(clientId.ToString(), token)).ReturnsAsync(clientId.ToString());
            _orderRepositoryMock.Setup(m => m.ListAsync(It.IsAny<Expression<Func<Order, bool>>>(), token)).ReturnsAsync(null as List<Order>);

            //Act
            var result = () => _handler.Handle(new(clientId), token);

            //Assert
            result.Should().ThrowAsync<NotFoundException>().WithMessage(errorMsg);
            _distributedCacheMock.Verify(m => m.GetAsync(It.IsAny<string>(), token), Times.Once);
            _userServiceMock.Verify(m => m.GetByIdAsync(clientId.ToString(), token), Times.Once);
            _orderRepositoryMock.Verify(m => m.ListAsync(It.IsAny<Expression<Func<Order, bool>>>(), token), Times.Once);
            _distributedCacheMock.Verify(m => m.SetAsync(
                It.IsAny<string>(),
                It.IsAny<byte[]>(),
                It.IsAny<DistributedCacheEntryOptions>(), token),
                Times.Never);
        }

        [Fact]
        public async Task Handle_OrdersExistInDb_ReturnsData()
        {
            //Arrange
            var orders = Enumerable.Range(1, 10).Select(i => TestDataProvider.SampleOrderEntity).ToList();
            var ordersDTO = _mapper.Map<List<OrderResponseDTO>>(orders);
            var serializedDTO = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(ordersDTO));
            var clientId = Guid.NewGuid();
            var token = CancellationToken.None;

            _distributedCacheMock.Setup(m => m.GetAsync(It.IsAny<string>(), token)).ReturnsAsync(null as byte[]);
            _userServiceMock.Setup(m => m.GetByIdAsync(clientId.ToString(), token)).ReturnsAsync(clientId.ToString());
            _orderRepositoryMock.Setup(m => m.ListAsync(It.IsAny<Expression<Func<Order, bool>>>(), token)).ReturnsAsync(orders);

            //Act
            var result = await _handler.Handle(new(clientId), token);

            //Assert
            result.Should().BeEquivalentTo(ordersDTO);
            _distributedCacheMock.Verify(m => m.GetAsync(It.IsAny<string>(), token), Times.Once);
            _userServiceMock.Verify(m => m.GetByIdAsync(clientId.ToString(), token), Times.Once);
            _orderRepositoryMock.Verify(m => m.ListAsync(It.IsAny<Expression<Func<Order, bool>>>(), token), Times.Once);
            _distributedCacheMock.Verify(m => m.SetAsync(
                It.IsAny<string>(),
                It.IsAny<byte[]>(),
                It.IsAny<DistributedCacheEntryOptions>(), token),
                Times.Once);
        }
    }
}
