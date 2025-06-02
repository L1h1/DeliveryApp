using AutoMapper;
using OrderService.Application.DTOs.Request;
using OrderService.Domain.Entities;

namespace OrderService.Application.Mapping
{
    public class OrderItemProfile : Profile
    {
        public OrderItemProfile()
        {
            CreateMap<OrderItemRequestDTO, OrderItem>();
        }
    }
}
