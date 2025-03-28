using AutoMapper;
using EntityLayer.DTOs.SongDtos;
using EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Mappers
{
	public class SongProfile:Profile
	{
		public SongProfile()
		{
			CreateMap<Song, SongDto>().ReverseMap() ;
		}
	}
}
