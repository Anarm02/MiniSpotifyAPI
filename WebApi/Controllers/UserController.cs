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

		[HttpPost("change-role")]
		public async Task<IActionResult> ChangeRole([FromBody] ChangeUserRoleDto model)
		{
			if (model == null || string.IsNullOrEmpty(model.UserId) || model.NewRoles == null || model.NewRoles.Count == 0)
			{
				return BadRequest("Lütfən, müvafiq məlumatları daxil edin.");
			}

			try
			{
				var result = await _userService.ChangeUserRolesAsync(model.UserId, model.NewRoles);
				if (result)
					return Ok(new { message = "İstifadəçinin rolları uğurla yeniləndi." });

				return BadRequest("Rolların yenilənməsi zamanı xəta baş verdi.");
			}
			catch (UnauthorizedAccessException ex)
			{
				return Forbid(ex.Message);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpGet]
		[Authorize(Roles ="Admin")]
		public async Task<IActionResult> GetAllUsers()
		{
			var users = await _userService.GetAllUsers();
			return Ok(users);
		}

		[Authorize]
		[HttpPut]
		public async Task<IActionResult> EditUser(EditUserDto editUser)
		{
			var result = await _userService.EditUser(editUser);
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

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteUser(string id)
		{
			try
			{
				await _userService.RemoveUser(id);
				return Ok("Istifadeci silindi");
			}
			catch (Exception ex)
			{

				return BadRequest($"{ex.Message}\n ${ex.StackTrace}");
			}
		}
	}
}
