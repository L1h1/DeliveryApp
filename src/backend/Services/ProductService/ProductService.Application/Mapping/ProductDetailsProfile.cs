using AutoMapper;
using ProductService.Application.DTOs.Request;
using ProductService.Application.DTOs.Response;
using ProductService.Domain.Entities;

namespace ProductService.Application.Mapping
{
    public class ProductDetailsProfile : Profile
    {
        public ProductDetailsProfile()
        {
            CreateMap<ProductDetailsRequestDTO, ProductDetails>();
            CreateMap<ProductDetails, ProductDetailsResponseDTO>();
        }
    }
}
