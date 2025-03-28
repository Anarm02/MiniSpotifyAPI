using EntityLayer.DTOs.UserDtos;
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
	}
}
