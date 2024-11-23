using AutoMapper;
using ShoppingStore.Model;
using ShoppingStore.Model.Dtos;
using ShoppingStore.Model.Entities;

namespace ShoppingStore.API.Profiles
{
	public class UserProfile : Profile
	{
        public UserProfile()
        {
			CreateMap<User, UserDto>();
            CreateMap<UserClaim, UserClaimDto>();
            CreateMap<UserForEditDto, User>()
                .ForMember(m => m.Password, options => options.Ignore());



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
