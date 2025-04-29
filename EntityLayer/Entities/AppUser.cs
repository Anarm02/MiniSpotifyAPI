using EntityLayer.Entities.Common;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Entities
{
	public class AppUser : IdentityUser<Guid>,IEntityBase
	{
		public string FullName { get; set; }
		public string? RefreshToken { get; set; }
		public DateTime? RefreshTokenExpiration { get; set; }
		public bool? IsEmailVerified { get; set; }
		public ICollection<Song> Songs { get; set; }
		public ICollection<Playlist> Playlists { get; set; } = new List<Playlist>();
		public Guid? ProfileImageId { get; set; }   
		public Photo ProfileImage { get; set; }
	}
}
