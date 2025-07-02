using AutoMapper;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using ProductService.Application.DTOs.Response;
using ProductService.Application.Exceptions;
using ProductService.Application.Interfaces.Repositories;

namespace ProductService.Application.Commands.Product.UpdateProduct
{
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, ProductResponseDTO>
    {
        private readonly IMapper _mapper;
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IManufacturerRepository _manufacturerRepository;
        private readonly IDistributedCache _distributedCache;

        public UpdateProductCommandHandler(
            IMapper mapper,
            IProductRepository productRepository,
            ICategoryRepository categoryRepository,
            IManufacturerRepository manufacturerRepository,
            IDistributedCache distributedCache)
        {
            _mapper = mapper;
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _manufacturerRepository = manufacturerRepository;
            _distributedCache = distributedCache;
        }

        public async Task<ProductResponseDTO> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var cacheKey = $"product:{request.Id}";

            var existingProduct = await _productRepository.GetByIdAsync(request.Id, cancellationToken);

            if (existingProduct is null)
            {
                throw new NotFoundException("Product with given id not found.");
            }

            await UpdateCategoriesIfChangedAsync(existingProduct, request.RequestDTO.CategoryIds, cancellationToken);
            await UpdateManufacturerIfChangedAsync(existingProduct, request.RequestDTO.ManufacturerId, cancellationToken);

            _mapper.Map(request.RequestDTO, existingProduct);

            existingProduct = await _productRepository.UpdateAsync(existingProduct, cancellationToken);

            await _distributedCache.RemoveAsync(cacheKey, cancellationToken);

            return _mapper.Map<ProductResponseDTO>(existingProduct);
        }

        private async Task UpdateManufacturerIfChangedAsync(Domain.Entities.Product product, Guid newManufacturerId, CancellationToken cancellationToken)
        {
            if (product.ManufacturerId != newManufacturerId)
            {
                var manufacturer = await _manufacturerRepository.GetByIdAsync(newManufacturerId, cancellationToken);

                if (manufacturer is null)
                {
                    throw new NotFoundException("Manufacturer not found");
                }

                product.Manufacturer = manufacturer;
            }
        }

        private async Task UpdateCategoriesIfChangedAsync(Domain.Entities.Product product, List<Guid> requestedCategoryIds, CancellationToken cancellationToken)
        {
            var currentCategoryIds = product.Categories.Select(c => c.Id).ToHashSet();
            var requestedIdsSet = requestedCategoryIds.ToHashSet();

            if (!currentCategoryIds.SetEquals(requestedIdsSet))
            {
                var categories = await _categoryRepository.ListByIdsAsync(requestedCategoryIds, cancellationToken);

                if (categories.Count < requestedCategoryIds.Count)
                {
                    throw new NotFoundException("Some of the categories not found.");
                }

                product.Categories = categories.ToList();
            }
        }
    }
}
