using EntityLayer.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Entities
{
	public class Playlist:EntityBase
	{
		public string Name { get; set; }
		public Guid UserId { get; set; }
		public AppUser User { get; set; }
		public ICollection<Song> Songs { get; set; } = new List<Song>();
		public TimeSpan Duration { get; set; } = TimeSpan.Zero;
		public void ReCalculation()
		{
			Duration=new TimeSpan(Songs.Sum(s=>s.Duration.Ticks));
		}
		public Guid? CoverImageId { get; set; }
		public Photo CoverImage { get; set; }
	}
}
