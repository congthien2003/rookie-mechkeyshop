using AutoMapper;
using Domain.Entity;
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
        }
    }
}