using AutoMapper;
using EntityLayer.DTOs.UserDtos;
using EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Mappers
{
	public class UserProfile : Profile
	{
		public UserProfile()
		{
			CreateMap<UserDto, AppUser>().ReverseMap();
			CreateMap<EditUserDto, AppUser>().ReverseMap();
			CreateMap<EditUserDto, UserDto>().ReverseMap();
		}
	}
}
