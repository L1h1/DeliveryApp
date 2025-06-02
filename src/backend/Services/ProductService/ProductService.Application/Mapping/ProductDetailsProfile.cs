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
            CreateMap<ProductDetailsRequestDTO, ProductDetails>()
                .ForMember(dest => dest.Id, src => src.MapFrom(src => src.ProductId));

            CreateMap<ProductDetails, DetailedProductResponseDTO>();

            CreateMap<ProductDetails, ProductDetailsResponseDTO>()
                .ForMember(dest => dest.ProductId, src => src.MapFrom(src => src.Id));
        }
    }
}
