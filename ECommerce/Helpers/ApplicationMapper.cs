using AutoMapper;
using ECommerce.Data;
using ECommerce.Models.Auth;
using ECommerce.Models.Category;
using ECommerce.Models.Domain;
using ECommerce.Models.Order;
using ECommerce.Models.Product;

namespace ECommerce.Helpers
{
    public class ApplicationMapper : Profile
    {
        public ApplicationMapper()
        {
            CreateMap<Category, CategoryViewModel>().ReverseMap();
            CreateMap<Category, CategoryModel>().ReverseMap();

            CreateMap<Product, ProductModel>().ReverseMap();
            CreateMap<Product, ProductViewModel>()
                .ForMember(m => m.Category, o => o.MapFrom(e => e.Category == null ? "" : e.Category.Name))
                .ForMember(m => m.Image, o => o.MapFrom(e => string.IsNullOrEmpty(e.Image) ? null : $"http://localhost:5085/Resources/{e.Image}"))
                .ReverseMap();

            CreateMap<RegisterModel, ApplicationUser>();

            CreateMap<Order, OrderModel>().ReverseMap();
            CreateMap<Order, OrderViewModel>().ReverseMap();
            CreateMap<ProductOrder, ProductOrderViewModel>().ReverseMap();

            CreateMap<ApplicationUser, UserModel>().ReverseMap();
        }
    }
}
