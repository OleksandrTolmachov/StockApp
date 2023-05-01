using AutoMapper;
using Domain.Models;
using StockApp.Application.DTO;

namespace StockApp.Infrastructure.Mappings;

public class OrderProfile : Profile
{
    public OrderProfile()
    {
        CreateMap<OrderResponse, OrderPDF>().ForMember(
            dest => dest.OrderType,
            opt => opt.MapFrom(src => OrdersType.Buy));

        CreateMap<OrderResponse, OrderPDF>().ForMember(
            dest => dest.OrderType,
            opt => opt.MapFrom(src => OrdersType.Sell));

        CreateMap<IOrderRequest, SellOrder>();
        CreateMap<IOrderRequest, BuyOrder>();
        CreateMap<IOrder, OrderResponse>();
    }
}
