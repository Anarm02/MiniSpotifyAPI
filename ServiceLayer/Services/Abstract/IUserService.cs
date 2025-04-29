using EntityLayer.DTOs.UserDtos;
using EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Services.Abstract
{
	public interface IUserService
	{
		Task<List<UserDto>> GetAllUsers();
		Task<UserDto> EditUser(EditUserDto editUser);
		Task<bool> ChangeUserRolesAsync(string userId, List<string> newRoles);
		Task RemoveUser(string userId);
		Task<List<string>> GetAllRolesAsync();
	}
}
