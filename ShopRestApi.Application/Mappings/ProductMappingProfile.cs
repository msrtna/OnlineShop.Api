using AutoMapper;
using ShopRestApi.Application.DTOs.ProductsDtos;
using ShopRestApi.Domain.Entities;

namespace ShopRestApi.Application.Mappings
{
    public class ProductMappingProfile : Profile
    {
        public ProductMappingProfile()
        {
            CreateMap<Product, ProductDto>();
            CreateMap<CreateProductDto, Product>();
            CreateMap<UpdateProductDto, Product>().ReverseMap();
        }
    }
}
