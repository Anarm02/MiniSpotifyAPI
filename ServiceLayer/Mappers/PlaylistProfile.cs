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
			CreateMap<PlaylistDto ,Playlist>().ReverseMap();
			CreateMap<UpdatePlaylistDto ,Playlist>().ReverseMap();
		}
	}
}
