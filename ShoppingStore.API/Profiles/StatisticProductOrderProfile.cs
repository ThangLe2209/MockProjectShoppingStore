using AutoMapper;
using ShoppingStore.Model;
using ShoppingStore.Model.Dtos;

namespace ShoppingStore.API.Profiles
{
	public class StatisticProductOrderProfile : Profile
	{
        public StatisticProductOrderProfile()
        {
			CreateMap<StatisticalProductOrderModel, StatisticalProductOrderDto>();
			CreateMap<StatisticalProductOrderForCreationDto, StatisticalProductOrderModel>();
			//CreateMap<CouponModel, CouponForEditDto>();
			//CreateMap<CouponForEditDto, CouponModel>();



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
