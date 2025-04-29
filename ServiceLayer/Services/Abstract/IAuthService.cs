using EntityLayer.DTOs.AuthDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Services.Abstract
{
	public interface IAuthService
	{
		Task<string> Register(RegisterDto registerDto);
		Task<LoginResponseDto> LoginAsync(LoginDto model);
		Task<string> VerifyEmailAsync(string userId, string token);
		Task LogoutAsync();
		
	}
}
