using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Moq;
using OrderService.Application.Commands.CreateOrder;
using OrderService.Application.DTOs.Response;
using OrderService.Application.Exceptions;
using OrderService.Application.Interfaces.Repositories;
using OrderService.Application.Interfaces.Services;
using OrderService.Domain.Entities;

namespace OrderService.Tests.UnitTests.Commands
{
    public class CreateOrderCommandHandlerTests
    {
        private readonly IMapper _mapper;
        private readonly Mock<IOrderRepository> _orderRepositoryMock;
        private readonly Mock<IProductService> _productServiceMock;
        private readonly Mock<IUserService> _userServiceMock;
        private readonly Mock<IOrderFlowService> _orderFlowServiceMock;
        private readonly Mock<IDistributedCache> _distributedCacheMock;
        private readonly Mock<ILogger<CreateOrderCommandHandler>> _loggerMock;
        private readonly CreateOrderCommandHandler _handler;

        public CreateOrderCommandHandlerTests()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddMaps(typeof(CreateOrderCommandHandler).Assembly);
            });
            _mapper = new Mapper(config);

            _orderFlowServiceMock = new();
            _orderRepositoryMock = new();
            _productServiceMock = new();
            _userServiceMock = new();
            _distributedCacheMock = new();
            _loggerMock = new();

            _handler = new(
                _mapper,
                _orderRepositoryMock.Object,
                _productServiceMock.Object,
                _userServiceMock.Object,
                _distributedCacheMock.Object,
                _loggerMock.Object,
                _orderFlowServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ClientDoesNotExist_ThrowsNotFoundException()
        {
            //Arrange
            var order = TestDataProvider.SampleOrderRequestDTO;
            var token = CancellationToken.None;
            var errorMsg = "Client with given id not found.";
            var clientId = order.ClientId;

            _userServiceMock.Setup(m => m.GetByIdAsync(clientId.ToString(), token)).ReturnsAsync(null as string);

            //Act
            var result = () => _handler.Handle(new (order), token);

            //Assert
            result.Should().ThrowAsync<NotFoundException>().WithMessage(errorMsg);
            _userServiceMock.Verify(m => m.GetByIdAsync(clientId.ToString(), token), Times.Once);
            _productServiceMock.Verify(m => m.GetProductsAsync(It.IsAny<List<Guid>>(), token), Times.Never);
            _orderRepositoryMock.Verify(m => m.CreateAsync(It.IsAny<Order>(), token), Times.Never);
            _orderFlowServiceMock.Verify(m => m.ProcessOrderCreation(It.IsAny<Order>(), It.IsAny<string>(), token), Times.Never);
            _distributedCacheMock.Verify(m => m.RemoveAsync(It.IsAny<string>(), token), Times.Never);
        }

        [Fact]
        public async Task Handle_ProductsDoNotExist_ThrowsNotFoundException()
        {
            //Arrange
            var order = TestDataProvider.SampleOrderRequestDTO;
            var token = CancellationToken.None;
            var errorMsg = "Some products where not found.";
            var clientId = order.ClientId;
            var clientEmail = "test@mail.com";

            _userServiceMock.Setup(m => m.GetByIdAsync(clientId.ToString(), token)).ReturnsAsync(clientEmail);
            _productServiceMock.Setup(m => m.GetProductsAsync(It.IsAny<List<Guid>>(), token)).ReturnsAsync(new List<ProductResponseDTO>());

            //Act
            var result = () => _handler.Handle(new(order), token);

            //Assert
            result.Should().ThrowAsync<NotFoundException>().WithMessage(errorMsg);
            _userServiceMock.Verify(m => m.GetByIdAsync(clientId.ToString(), token), Times.Once);
            _productServiceMock.Verify(m => m.GetProductsAsync(It.IsAny<List<Guid>>(), token), Times.Once);
            _orderRepositoryMock.Verify(m => m.CreateAsync(It.IsAny<Order>(), token), Times.Never);
            _orderFlowServiceMock.Verify(m => m.ProcessOrderCreation(It.IsAny<Order>(), clientEmail, token), Times.Never);
            _distributedCacheMock.Verify(m => m.RemoveAsync(It.IsAny<string>(), token), Times.Never);
        }

        [Fact]
        public async Task Handle_ValidDataProvided_SuccessfulOrderCreation()
        {
            //Arrange
            var orderRequestDTO = TestDataProvider.SampleOrderRequestDTO;
            var productList = orderRequestDTO.Items.Select(x => new ProductResponseDTO()).ToList();
            var order = _mapper.Map<Order>(orderRequestDTO);
            var token = CancellationToken.None;
            var errorMsg = "Some products where not found.";
            var clientId = orderRequestDTO.ClientId;
            var clientEmail = "test@mail.com";

            _userServiceMock.Setup(m => m.GetByIdAsync(clientId.ToString(), token)).ReturnsAsync(clientEmail);
            _productServiceMock.Setup(m => m.GetProductsAsync(It.IsAny<List<Guid>>(), token)).ReturnsAsync(productList);
            _orderRepositoryMock.Setup(m => m.CreateAsync(order, token)).ReturnsAsync(order);
            _orderFlowServiceMock.Setup(m => m.ProcessOrderCreation(order, clientEmail, token)).Returns(Task.CompletedTask);
            _distributedCacheMock.Setup(m => m.RemoveAsync(It.IsAny<string>(), token)).Returns(Task.CompletedTask);

            //Act
            var result = await _handler.Handle(new(orderRequestDTO), token);

            //Assert
            result.Should().BeEquivalentTo(_mapper.Map<OrderResponseDTO>(order), options => options.Excluding(o => o.CreatedAt));
            _userServiceMock.Verify(m => m.GetByIdAsync(clientId.ToString(), token), Times.Once);
            _productServiceMock.Verify(m => m.GetProductsAsync(It.IsAny<List<Guid>>(), token), Times.Once);
            _orderRepositoryMock.Verify(m => m.CreateAsync(It.IsAny<Order>(), token), Times.Once);
            _orderFlowServiceMock.Verify(m => m.ProcessOrderCreation(It.IsAny<Order>(), clientEmail, token), Times.Once);
            _distributedCacheMock.Verify(m => m.RemoveAsync(It.IsAny<string>(), token), Times.Once);

        }
    }
}
