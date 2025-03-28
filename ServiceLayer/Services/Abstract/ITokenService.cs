using EntityLayer.DTOs.AuthDtos;
using EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Services.Abstract
{
	public interface ITokenService
	{
		TokenResponseDto GenerateToken(AppUser appUser, IList<string> roles);
		string GenerateRefreshToken();
		ClaimsPrincipal? GetPrincipal(string? token);

	}
}
