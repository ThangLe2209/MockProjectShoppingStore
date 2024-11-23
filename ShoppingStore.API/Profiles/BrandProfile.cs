using AutoMapper;
using ShoppingStore.Model;
using ShoppingStore.Model.Dtos;

namespace ShoppingStore.API.Profiles
{
	public class BrandProfile : Profile
	{
        public BrandProfile()
        {
            CreateMap<BrandModel, BrandDto>();
            CreateMap<BrandModel, BrandWithoutProductDto>();
			CreateMap<BrandForCreationDto, BrandModel>();
			CreateMap<BrandForEditDto, BrandModel>();



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
