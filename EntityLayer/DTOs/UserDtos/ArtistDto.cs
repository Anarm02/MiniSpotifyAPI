using EntityLayer.DTOs.SongDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.DTOs.UserDtos
{
	public class ArtistDto
	{
		public Guid Id { get; set; }
		public string FullName { get; set; }
		public List<SongDto> Songs { get; set; }
	}
}
