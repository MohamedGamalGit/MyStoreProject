using AutoMapper;
using Commen.ViewModels;
using Commen.ViewModels.RolesVM;
using Models.Models;
using Models.Models.Permission;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.AutoMapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // من Entity إلى ViewModel
            CreateMap<User, UserCreateVM>().ReverseMap();
            CreateMap<User, UserViewModel>()
                .ForMember(
                            dest => dest.UserRoles,
                            opt => opt.MapFrom(
                                src => src.UserRoles != null
                                    ? src.UserRoles.Select(ur => ur.Role.Name).ToList()
                                    : new List<string>()
                            )
                        )


                .ReverseMap();
            CreateMap<Product, ProductAddVM>().ReverseMap();
            CreateMap<Category, CategoryAddVM>().ReverseMap();
            CreateMap<Role, RoleAddVM>().ReverseMap();
            CreateMap<RolePageAction, RoleAssignDto>().ReverseMap();
            CreateMap<Page, PageDto>().ReverseMap();
        }
    }
}
