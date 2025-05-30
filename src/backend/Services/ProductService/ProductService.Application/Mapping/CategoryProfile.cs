using AutoMapper;
using ProductService.Application.DTOs.Request;
using ProductService.Application.DTOs.Response;
using ProductService.Domain.Entities;

namespace ProductService.Application.Mapping
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<Category, CategoryResponseDTO>();

            CreateMap<CategoryRequestDTO, Category>()
                .ForMember(dest => dest.NormalizedName, dest => dest.MapFrom(src => src.Name.ToLower()));
        }
    }
}
