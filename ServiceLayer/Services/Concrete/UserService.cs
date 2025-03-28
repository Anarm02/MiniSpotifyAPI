using AutoMapper;
using EntityLayer.DTOs.UserDtos;
using EntityLayer.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ServiceLayer.Services.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Services.Concrete
{
	public class UserService : IUserService
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly IHttpContextAccessor _contextAccessor;
		private readonly IMapper mapper;
		public UserService(UserManager<AppUser> userManager, IHttpContextAccessor contextAccessor, IMapper mapper)
		{
			_userManager = userManager;
			_contextAccessor = contextAccessor;
			this.mapper = mapper;
		}

		public async Task<List<UserDto>> GetAllUsers()
		{
			var currentUser = await _userManager.GetUserAsync(_contextAccessor.HttpContext.User);
			var users = await _userManager.Users.Where(u => u.Id != currentUser.Id)
			.Select(user => new UserDto
			{
				Id = user.Id,
				FullName = user.FullName,
				Email = user.Email,
				UserName = user.UserName,
				PhoneNumber=user.PhoneNumber
			}).ToListAsync();

			return users;
		}
		public async Task<UserDto> EditUser(EditUserDto editUser)
		{
			var currentUser = await _userManager.GetUserAsync(_contextAccessor.HttpContext.User);
			mapper.Map(editUser, currentUser);
			var map = mapper.Map<UserDto>(currentUser);
			return map;
		}
		
	}
}
