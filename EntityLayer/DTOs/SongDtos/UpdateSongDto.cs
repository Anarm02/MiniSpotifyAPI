using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.DTOs.SongDtos
{
	public class UpdateSongDto
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
	}
}
