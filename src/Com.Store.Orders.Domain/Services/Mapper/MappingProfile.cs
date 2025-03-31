using AutoMapper;
using Com.Store.Orders.Domain.Data.Entities;
using Com.Store.Orders.Domain.Data.Models;
using Com.Store.Orders.Domain.Data.Models.Pagination;
using Com.Store.Orders.Domain.Services.Dto;
using Com.Store.Orders.Domain.Services.Mapper.Converters;

namespace Com.Store.Orders.Domain.Services.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateOrderDto, Order>().ConvertUsing<CreateOrderDtoToOrderConverter>();
            CreateMap<Order, OrderDto>();
            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Item.Title))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Item.Description))
                .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.Item.Image))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Item.Price))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ItemId))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity));
            CreateMap<PageModel<Order>, PageModel<OrderDto>>();
            CreateMap<UserModel, UserDto>();
        }
    }
}
