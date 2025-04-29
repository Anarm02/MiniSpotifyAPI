using EntityLayer.Entities.Common;
using EntityLayer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Entities
{
	public class Photo:EntityBase
	{
		public string Url { get; set; }
		public ImageType ImageType { get; set; }
		public Guid? UserId { get; set; }
		public AppUser User { get; set; }

		public Guid? SongId { get; set; }
		public Song Song { get; set; }

		public Guid? PlaylistId { get; set; }
		public Playlist Playlist { get; set; }
	}
}
