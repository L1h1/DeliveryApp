using AutoMapper;
using OrderService.Application.DTOs.Response;
using OrderService.Application.Protos;

namespace OrderService.Application.Mapping
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<ProductResponse, ProductResponseDTO>();
        }
    }
}
