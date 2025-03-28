using EntityLayer.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Context
{
	public class AppDbContext:IdentityDbContext<AppUser,AppRole,Guid>
	{
		public AppDbContext(DbContextOptions options) : base(options)
		{
		}

		public DbSet<AppUser> Users { get; set; }
		public DbSet<AppRole> Roles { get; set; }
		public DbSet<Song> Songs { get; set; }
		public DbSet<Playlist> Playlists { get; set; }
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<Song>()
				.HasMany(s => s.Artists)
				.WithMany(a => a.Songs)
				.UsingEntity<Dictionary<string, object>>(
					"SongArtist",
					r => r.HasOne<AppUser>().WithMany().HasForeignKey("ArtistId"),
					l => l.HasOne<Song>().WithMany().HasForeignKey("SongId"));
		}

	}
}
