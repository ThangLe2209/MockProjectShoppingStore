﻿using AutoMapper;
using ShoppingStore.Model;
using ShoppingStore.Model.Dtos;

namespace ShoppingStore.API.Profiles
{
	public class SongProfile : Profile
	{
        public SongProfile()
        {
            CreateMap<SongModel, SongDto>();
			CreateMap<SongForCreationDto, SongModel>();
			CreateMap<SongForEditDto, SongModel>()
				.ForMember(m => m.Image, options => options.Ignore()) // if allow then noimages.jpg will ovveride current song
				.ForMember(m => m.Song, options => options.Ignore()); // if allow then String.Empty will ovveride current song




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
