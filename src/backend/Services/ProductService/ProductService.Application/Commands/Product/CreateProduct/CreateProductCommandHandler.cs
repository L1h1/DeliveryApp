using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ProductService.Application.DTOs.Response;
using ProductService.Application.Exceptions;
using ProductService.Application.Interfaces.Repositories;

namespace ProductService.Application.Commands.Product.CreateProduct
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductResponseDTO>
    {
        private readonly IMapper _mapper;
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IManufacturerRepository _manufacturerRepository;
        private readonly ILogger<CreateProductCommandHandler> _logger;

        public CreateProductCommandHandler(
            IMapper mapper,
            IProductRepository productRepository,
            ICategoryRepository categoryRepository,
            IManufacturerRepository manufacturerRepository,
            ILogger<CreateProductCommandHandler> logger)
        {
            _mapper = mapper;
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _manufacturerRepository = manufacturerRepository;
            _logger = logger;
        }

        public async Task<ProductResponseDTO> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var product = _mapper.Map<Domain.Entities.Product>(request.RequestDTO);

            _logger.LogInformation("Retrieving provided category list");

            var categories = await _categoryRepository.ListByIdsAsync(request.RequestDTO.CategoryIds, cancellationToken);

            if (categories.Count < request.RequestDTO.CategoryIds.Count)
            {
                throw new NotFoundException("Some of the categories not found.");
            }

            product.Categories = categories.ToList();

            _logger.LogInformation("Retrieving manufacturer @{id} data", request.RequestDTO.ManufacturerId);

            var existingManufacturer = await _manufacturerRepository.GetByIdAsync(request.RequestDTO.ManufacturerId, cancellationToken);

            if (existingManufacturer is null)
            {
                throw new NotFoundException("Manufacturer with given id not found.");
            }

            product.Manufacturer = existingManufacturer;

            product = await _productRepository.AddAsync(product, cancellationToken);

            _logger.LogInformation("Successfully created procuct @{id}", product.Id);

            return _mapper.Map<ProductResponseDTO>(product);
        }
    }
}
