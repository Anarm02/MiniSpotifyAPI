using EntityLayer.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Entities
{
	public class Song:EntityBase
	{
		public string Title { get; set; }
		public string FilePath { get; set; }
		public TimeSpan Duration { get; set; }
		public ICollection<AppUser> Artists { get; set; } = new List<AppUser>();
		public ICollection<Playlist> Playlists { get; set; } = new List<Playlist>();
		public Guid? ArtworkImageId { get; set; }
		public Photo ArtworkImage { get; set; }
	}
}
