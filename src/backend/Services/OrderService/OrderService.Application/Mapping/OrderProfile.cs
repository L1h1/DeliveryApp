using AutoMapper;
using OrderService.Application.DTOs.Request;
using OrderService.Application.DTOs.Response;
using OrderService.Domain.Entities;
using OrderService.Domain.Enums;

namespace OrderService.Application.Mapping
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            CreateMap<OrderRequestDTO, Order>()
                .ForMember(dest => dest.CreatedAt, dest => dest.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.OrderStatus, dest => dest.MapFrom(src => OrderStatus.Created));

            CreateMap<Order, OrderResponseDTO>()
                .ForMember(dest => dest.OrderStatus, dest => dest.MapFrom(src => src.OrderStatus.ToString()));

            CreateMap<Order, DetailedOrderResponseDTO>()
                .ForMember(dest => dest.OrderStatus, dest => dest.MapFrom(src => src.OrderStatus.ToString()));
        }
    }
}
