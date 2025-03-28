using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.DTOs.AuthDtos
{
	public class TokenResponseDto
	{
		public string? AccessToken { get; set; }
		public DateTime? TokenExpirationDate { get; set; }
		public string? RefreshToken { get; set; }
		public DateTime? RefreshTokenExpirationDate { get; set; }
	}
}
