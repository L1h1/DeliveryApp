using AutoMapper;
using ProductService.Application.DTOs.Request;
using ProductService.Application.DTOs.Response;
using ProductService.Domain.Entities;

namespace ProductService.Application.Mapping
{
    public class ManufacturerProfile : Profile
    {
        public ManufacturerProfile()
        {
            CreateMap<Manufacturer, ManufacturerResponseDTO>();

            CreateMap<ManufacturerRequestDTO, Manufacturer>()
                .ForMember(dest => dest.NormalizedName, dest => dest.MapFrom(src => src.Name.ToLower()));
        }
    }
}
