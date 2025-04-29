using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.DTOs.SongDtos
{
	public class CreateSongDto
	{
		public IFormFile MusicFile { get; set; }
		public string Title { get; set; }
		public IEnumerable<string>? additionalArtistNames { get; set; }=null;
		public IFormFile? ArtworkFile { get; set; }

	}
}
