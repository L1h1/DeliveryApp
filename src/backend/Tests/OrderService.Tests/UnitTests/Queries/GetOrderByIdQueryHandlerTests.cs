using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using System.Text.Json;
using OrderService.Application.DTOs.Response;
using OrderService.Application.Interfaces.Repositories;
using OrderService.Application.Queries.GetOrderById;
using System.Text;
using OrderService.Domain.Entities;
using OrderService.Application.Exceptions;
using Castle.Core.Logging;
using Microsoft.Extensions.Logging;

namespace OrderService.Tests.UnitTests.Queries
{
    public class GetOrderByIdQueryHandlerTests
    {
        private readonly Mock<IOrderRepository> _orderRepositoryMock;
        private readonly Mock<IDistributedCache> _distributedCacheMock;
        private readonly IMapper _mapper;
        private readonly Mock<ILogger<GetOrderByIdQueryHandler>> _loggerMock;
        private readonly GetOrderByIdQueryHandler _getOrderByIdQueryHandler;

        public GetOrderByIdQueryHandlerTests()
        {
            _orderRepositoryMock = new();
            _distributedCacheMock = new();
            _loggerMock = new();
            
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddMaps(typeof(GetOrderByIdQueryHandler).Assembly);
            });
            _mapper = new Mapper(config);

            _getOrderByIdQueryHandler = new(
                _orderRepositoryMock.Object,
                _mapper,
                _loggerMock.Object,
                _distributedCacheMock.Object);
        }

        [Fact]
        public async Task Handle_OrderExistsInCache_ReturnOrderData()
        {
            //Arrange
            var order = TestDataProvider.SampleOrderEntity;
            var orderDTO = _mapper.Map<DetailedOrderResponseDTO>(order);
            var serializedDTO = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(orderDTO));
            var orderId = order.Id;
            var token = CancellationToken.None;

            _distributedCacheMock.Setup(m => m.GetAsync(It.IsAny<string>(), token)).ReturnsAsync(serializedDTO);
            _orderRepositoryMock.Setup(m => m.GetByIdAsync(orderId, token)).ReturnsAsync(order);

            //Act
            var result = await _getOrderByIdQueryHandler.Handle(new(orderId), token);

            //Assert
            _distributedCacheMock.Verify(m => m.GetAsync(It.IsAny<string>(), token), Times.Once);
            _orderRepositoryMock.Verify(m => m.GetByIdAsync(orderId, token), Times.Never);
            _distributedCacheMock.Verify(m => m.SetAsync(
                It.IsAny<string>(),
                serializedDTO,
                It.IsAny<DistributedCacheEntryOptions>(), token),
                Times.Never);
            result.Should().BeEquivalentTo(orderDTO);
        }

        [Fact]
        public async Task Handle_OrderDoesNotExistInDb_ThrowNotFoundException()
        {
            //Arrange
            var orderId = Guid.NewGuid();
            var token = CancellationToken.None;

            _distributedCacheMock.Setup(m => m.GetAsync(It.IsAny<string>(), token)).ReturnsAsync(null as byte[]);
            _orderRepositoryMock.Setup(m => m.GetByIdAsync(orderId, token)).ReturnsAsync(null as Order);

            //Act
            var result = () => _getOrderByIdQueryHandler.Handle(new(orderId), token);

            //Assert
            result.Should().ThrowAsync<NotFoundException>();
            _distributedCacheMock.Verify(m => m.GetAsync(It.IsAny<string>(), token), Times.Once);
            _orderRepositoryMock.Verify(m => m.GetByIdAsync(orderId, token), Times.Once);
            _distributedCacheMock.Verify(m => m.SetAsync(
                It.IsAny<string>(),
                It.IsAny<byte[]>(),
                It.IsAny<DistributedCacheEntryOptions>(), token),
                Times.Never);
        }

        [Fact]
        public async Task Handle_OrderExistsInDb_ReturnOrderData()
        {
            //Arrange
            var order = TestDataProvider.SampleOrderEntity;
            var orderDTO = _mapper.Map<DetailedOrderResponseDTO>(order);
            var serializedDTO = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(orderDTO));
            var orderId = order.Id;
            var token = CancellationToken.None;

            _distributedCacheMock.Setup(m => m.GetAsync(It.IsAny<string>(), token)).ReturnsAsync(null as byte[]);
            _orderRepositoryMock.Setup(m => m.GetByIdAsync(orderId, token)).ReturnsAsync(order);

            //Act
            var result = await _getOrderByIdQueryHandler.Handle(new(orderId), token);

            //Assert
            _distributedCacheMock.Verify(m => m.GetAsync(It.IsAny<string>(), token), Times.Once);
            _orderRepositoryMock.Verify(m => m.GetByIdAsync(orderId, token), Times.Once);
            _distributedCacheMock.Verify(m => m.SetAsync(
                It.IsAny<string>(),
                serializedDTO,
                It.IsAny<DistributedCacheEntryOptions>(), token),
                Times.Once);
            result.Should().BeEquivalentTo(orderDTO);
        }
    }
}
