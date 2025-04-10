using EntityLayer.DTOs.AuthDtos;
using EntityLayer.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ServiceLayer.Services.Abstract;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Services.Concrete
{
	public class TokenService : ITokenService
	{
		private readonly IConfiguration _configuration;
		public TokenService(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public TokenResponseDto GenerateToken(AppUser appUser, IList<string> roles)
		{
			DateTime tokenExp = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpirationMinutes"]));
			DateTime refreshTokenExp = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:RefreshTokenValidity"]));


			List<Claim> claims = new List<Claim>
	{
		new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
		new Claim(JwtRegisteredClaimNames.Sub, appUser.Id.ToString()),
		new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString()),
		new Claim(ClaimTypes.Email, appUser.Email),
		new Claim(ClaimTypes.Name, appUser.FullName)
	};


			foreach (var role in roles)
			{
				claims.Add(new Claim(ClaimTypes.Role, role));
			}

			SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SigningKey"]));
			SigningCredentials credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

			JwtSecurityToken token = new JwtSecurityToken(
				issuer: _configuration["Jwt:Issuer"],
				audience: _configuration["Jwt:Audience"],
				claims: claims,
				expires: tokenExp,
				signingCredentials: credentials
			);

			JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
			string accessToken = handler.WriteToken(token);

			return new TokenResponseDto()
			{
				
				AccessToken = accessToken,
				TokenExpirationDate = tokenExp,
				RefreshTokenExpirationDate = refreshTokenExp,
				RefreshToken = GenerateRefreshToken()
			};
		}


		public string GenerateRefreshToken()
		{
			byte[] bytes = new byte[64];
			var rng = RandomNumberGenerator.Create();
			rng.GetBytes(bytes);
			return Convert.ToBase64String(bytes);
		}
		public ClaimsPrincipal? GetPrincipal(string? token)
		{
			var validationParameters = new TokenValidationParameters
			{
				ValidateIssuer = true,
				ValidIssuer = _configuration["Jwt:Issuer"],
				ValidateAudience = true,
				ValidAudience = _configuration["Jwt:Audience"],
				ValidateLifetime = false,
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SigningKey"])),

			};
			JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
			ClaimsPrincipal principal = handler.ValidateToken(token, validationParameters, out SecurityToken securityToken);
			if (securityToken is not JwtSecurityToken jwtSecurityToken || jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
			{
				throw new SecurityTokenException("Invalid token");
			}
			return principal;

		}
	}
}
