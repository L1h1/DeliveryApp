using AutoMapper;
using ProductService.Application.DTOs.Request;
using ProductService.Application.DTOs.Response;
using ProductService.Domain.Entities;

namespace ProductService.Application.Mapping
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<Product, ProductResponseDTO>()
                .ForMember(p => p.UnitOfMeasure, dest => dest.MapFrom(src => src.UnitOfMeasure.ToString()))
                .ForMember(p => p.Country, dest => dest.MapFrom(src => src.Manufacturer.Country))
                .ForMember(p => p.CategoryNames, dest => dest.MapFrom(src => src.Categories.Select(c => c.Name)));

            CreateMap<Product, DetailedProductResponseDTO>()
                .ForMember(p => p.UnitOfMeasure, dest => dest.MapFrom(src => src.UnitOfMeasure.ToString()))
                .ForMember(p => p.ManufacturerCountry, dest => dest.MapFrom(src => src.Manufacturer.Country))
                .ForMember(p => p.ManufacturerName, dest => dest.MapFrom(src => src.Manufacturer.Name))
                .ForMember(p => p.ManufacturerAddress, dest => dest.MapFrom(src => src.Manufacturer.Address));

            CreateMap<ProductDetailsProfile, DetailedProductResponseDTO>();

            CreateMap<ProductRequestDTO, Product>();
        }
    }
}
