using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.DTOs.PlaylistDtos
{
	public class CreatePlaylistDto
	{
		public string Name { get; set; }
		public IFormFile? ArtworkFile { get; set; }

	}
}
