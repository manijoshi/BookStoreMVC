

using AutoMapper;
using BookStore.Domain.Entities;
using BookStore.Domain.ViewModels;

namespace BookStore.Application.MappingProfiles
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<ProductViewModel, Product>();
            CreateMap<Product, ProductViewModel>();
        }
    }
}
