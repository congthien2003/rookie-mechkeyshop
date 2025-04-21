using AutoMapper;
using Domain.Entity;
using Domain.Enum;
using Newtonsoft.Json;
using Shared.ViewModels;
using Shared.ViewModels.Auth;

namespace Infrastructure.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<ApplicationUser, ApplicationUserModel>().ReverseMap();
            CreateMap<ApplicationUser, RegisterModel>().ReverseMap();
            CreateMap<Category, CategoryModel>().ReverseMap();

            CreateMap<Product, ProductModel>()
                .ForMember(p => p.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(p => p.Rating, opt => opt.MapFrom(src => src.ProductRatings))
                .ForMember(p => p.TotalRating,
                opt => opt.MapFrom(src => src.ProductRatings.Count() > 0 ? src.ProductRatings.Average(pr => pr.Stars) : 0))
                .ForMember(p => p.Variants, opt => opt.MapFrom(src => JsonConvert.DeserializeObject<List<VariantAttribute>>(src.Variants)))
                .ReverseMap();

            // Map VM to Entity to insert
            CreateMap<ProductRatingModel, ProductRating>()
                .ForMember(pr => pr.User, opt => opt.Ignore())
                .ForMember(pr => pr.Product, opt => opt.Ignore())
                .ReverseMap();

            // Map Entity to VM to show
            CreateMap<ProductRating, ProductRatingModel>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.User.Name))
                .ReverseMap();

            CreateMap<Order, OrderModel>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.User.Name))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderItems))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => ((OrderStatus)src.Status).ToString()))
                .ReverseMap();

            CreateMap<OrderItem, OrderItemModel>()
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Product.Price))
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.Product.ImageUrl))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.Option, opt => opt.MapFrom(src => JsonConvert.DeserializeObject<VariantAttribute>(src.Option)))
                .ReverseMap();

            CreateMap<OrderItem, CreateOrderItemViewModel>()
                .ForMember(dest => dest.Option, opt =>
                opt.MapFrom(src => JsonConvert.DeserializeObject<VariantAttribute>(src.Option)))
                .ReverseMap();

        }
    }
}