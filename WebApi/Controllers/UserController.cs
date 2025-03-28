using EntityLayer.DTOs.UserDtos;
using EntityLayer.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.Services.Abstract;

namespace WebApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UserController : ControllerBase
	{
		private readonly IUserService _userService;
		private readonly UserManager<AppUser> userManager;
		public UserController(IUserService userService, UserManager<AppUser> userManager)
		{
			_userService = userService;
			this.userManager = userManager;
		}
		[HttpGet]
		[Authorize(Policy ="AdminPolicy")]
		public async Task<IActionResult> GetAllUsers()
		{
			var users = await _userService.GetAllUsers();
			return Ok(users);
		}
		[Authorize]
		[HttpPut]
		public async Task<IActionResult> EditUser(EditUserDto editUser)
		{
			var result=await _userService.EditUser(editUser);
			return Ok(result);
		}
		[Authorize]
		[HttpGet("my-role")]
		public async Task<IActionResult> GetUserRole()
		{
			var user = await userManager.GetUserAsync(User);
			if (user == null) return Unauthorized();

			var roles = await userManager.GetRolesAsync(user);
			return Ok(new { Roles = roles });
		}
	}
}
