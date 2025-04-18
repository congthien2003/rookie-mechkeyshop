using AutoMapper;
using Domain.Entity;
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
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.User.Name));

        }
    }
}