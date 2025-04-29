using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.DTOs.PlaylistDtos
{
	public class PlaylistDto
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public TimeSpan Duration { get; set; }
		public string? ImagePath  { get; set; }
	}
}
