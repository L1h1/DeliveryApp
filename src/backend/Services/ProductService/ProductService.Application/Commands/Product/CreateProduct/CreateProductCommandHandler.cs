using AutoMapper;
using MediatR;
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

        public CreateProductCommandHandler(IMapper mapper, IProductRepository productRepository, ICategoryRepository categoryRepository, IManufacturerRepository manufacturerRepository)
        {
            _mapper = mapper;
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _manufacturerRepository = manufacturerRepository;
        }

        public async Task<ProductResponseDTO> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var product = _mapper.Map<Domain.Entities.Product>(request.requestDTO);

            var categories = await _categoryRepository.ListByIdsAsync(request.requestDTO.CategoryIds, cancellationToken);

            if (categories.Count < request.requestDTO.CategoryIds.Count)
            {
                throw new NotFoundException("Some of the categories not found.");
            }

            product.Categories = categories.ToList();

            var existingManufacturer = await _manufacturerRepository.GetByIdAsync(request.requestDTO.ManufacturerId, cancellationToken);

            if (existingManufacturer is null)
            {
                throw new NotFoundException("Manufacturer with given id not found.");
            }

            product.Manufacturer = existingManufacturer;

            product = await _productRepository.AddAsync(product, cancellationToken);

            return _mapper.Map<ProductResponseDTO>(product);
        }
    }
}
