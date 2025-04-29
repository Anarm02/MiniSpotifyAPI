using EntityLayer.DTOs.AuthDtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.Services.Abstract;
using ServiceLayer.Services.Concrete;

namespace WebApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly IAuthService authService;

		public AuthController(IAuthService authService)
		{
			this.authService = authService;
		}
		[HttpPost("Register")]
		public async Task<IActionResult> Register(RegisterDto registerDto)
		{
			var result = await authService.Register(registerDto);
			if (result.Contains("Qeydiyyat tamamlandı"))
				return Ok(result);

			return BadRequest(result);
		}

		[HttpGet("verify-email")]
		public async Task<IActionResult> VerifyEmail(string userId, string token)
		{
			var result = await authService.VerifyEmailAsync(userId, token);
			return result.Contains("uğurla") ? Ok(result) : BadRequest(result);
		}

		[HttpPost("Logout")]
		public async Task<IActionResult> Logout()
		{
			
				await authService.LogoutAsync();
				return Ok("Hesabdan ugurla cixildi");
			
		}

		[HttpPost("Login")]
		public async Task<IActionResult> Login(LoginDto loginDto)
		{
			var result = await authService.LoginAsync(loginDto);
			return Ok(result);
		}
	}
}
