using AutoMapper;
using EntityLayer.DTOs.PlaylistDtos;
using EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Mappers
{
	public class PlaylistProfile:Profile
	{
		public PlaylistProfile()
		{
			CreateMap<Playlist ,PlaylistDto>().ForMember(d => d.ImagePath,
					   o => o.MapFrom(s => s.CoverImage != null
											? s.CoverImage.Url
											: null)).ReverseMap();
			CreateMap<UpdatePlaylistDto ,Playlist>().ReverseMap();
		}
	}
}
