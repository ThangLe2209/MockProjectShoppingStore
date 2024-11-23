using AutoMapper;
using ShoppingStore.Model.Dtos;
using ShoppingStore.Model;

namespace ShoppingStore.API.Profiles
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<ProductModel, ProductDto>();
            CreateMap<ProductForCreationDto, ProductModel>();
            CreateMap<ProductForEditDto, ProductModel>()
                .ForMember(m => m.Image, options => options.Ignore())  // still get value from client but ignore when mapping field "Image" from ProductForEditDto to ProductModel
                //.ForMember(m => m.Brand, options => options.Ignore())
                //.ForMember(m => m.Category, options => options.Ignore())
                .ForMember(m => m.Ratings, options => options.Ignore());
            CreateMap<ProductModel, ProductWithoutWishlistRatingDto>();
            CreateMap<ProductModel, ProductWithoutBrandDto>();
            CreateMap<ProductModel, ProductWithoutProductQuantityDto>();
            CreateMap<ProductModel, ProductForEditWithQuantityDto>();
            CreateMap<ProductForEditWithQuantityDto, ProductModel>();




            //Example
            // map from Image (entity) to Image, and back
            //CreateMap<Entities.Image, Model.Image>().ReverseMap();

            // map from ImageForCreation to Image
            // Ignore properties that shouldn't be mapped
            //CreateMap<Model.ImageForCreation, Entities.Image>()
            //	.ForMember(m => m.FileName, options => options.Ignore())
            //	.ForMember(m => m.Id, options => options.Ignore())
            //	.ForMember(m => m.OwnerId, options => options.Ignore());
        }
    }
}
