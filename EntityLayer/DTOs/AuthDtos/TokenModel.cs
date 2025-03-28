using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.DTOs.AuthDtos
{
	public class TokenModel
	{
		public string AccessToken { get; set; }
		public string RefreshToken { get; set; }
	}
}
