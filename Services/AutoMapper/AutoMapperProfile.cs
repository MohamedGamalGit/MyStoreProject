using AutoMapper;
using Commen.ViewModels;
using Commen.ViewModels.RolesVM;
using Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.AutoMapper
{
    public class AutoMapperProfile:Profile
    {
        public AutoMapperProfile()
        {
            // من Entity إلى ViewModel
            CreateMap<User, UserCreateVM>().ReverseMap();
            CreateMap<User, UserViewModel>().ReverseMap();
            CreateMap<Product, ProductAddVM>().ReverseMap();
            CreateMap<Category, CategoryAddVM>().ReverseMap();
            CreateMap<Role, RoleAddVM>().ReverseMap();
        }
    }
}
