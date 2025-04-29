using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.DTOs.SongDtos
{
	public class SongDto
	{
		public Guid Id { get; set; }
		public string Title { get; set; }
		public string FilePath { get; set; }
		public TimeSpan Duration { get; set; }
		public List<string> ArtistNames { get; set; }
		public string? ImagePath { get; set; }
	}
}
