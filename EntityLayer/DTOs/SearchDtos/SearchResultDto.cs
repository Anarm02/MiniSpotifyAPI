using EntityLayer.DTOs.SongDtos;
using EntityLayer.DTOs.UserDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.DTOs.SearchDtos
{
	public class SearchResultDto
	{
		public List<SongDto> Songs { get; set; }
		public List<ArtistDto> Users { get; set; }
	}
}
