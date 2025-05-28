using AutoMapper;
using OrderService.Application.DTOs.Request;
using OrderService.Application.DTOs.Response;
using OrderService.Domain.Entities;

namespace OrderService.Application.Mapping
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            CreateMap<OrderRequestDTO, Order>();
            CreateMap<Order, OrderResponseDTO>();
            CreateMap<Order, DetailedOrderResponseDTO>();
        }
    }
}
