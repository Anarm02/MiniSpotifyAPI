using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.DTOs.AuthDtos
{
	public class LoginResponseDto
	{
		public string AccessToken { get; set; }
		public string RefreshToken { get; set; }
		public string UserName { get; set; }
		public string Roles { get; set; }
		public string UserId { get; set; }
	}
}
