using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
	public partial class Playlist_Tables : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
				name: "Playlists",
				columns: table => new
				{
					Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
					UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
					IsDeleted = table.Column<bool>(type: "bit", nullable: false),
					UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
					DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Playlists", x => x.Id);
					table.ForeignKey(
						name: "FK_Playlists_AspNetUsers_UserId",
						column: x => x.UserId,
						principalTable: "AspNetUsers",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "PlaylistSong",
				columns: table => new
				{
					PlaylistsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					SongsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_PlaylistSong", x => new { x.PlaylistsId, x.SongsId });
					table.ForeignKey(
						name: "FK_PlaylistSong_Playlists_PlaylistsId",
						column: x => x.PlaylistsId,
						principalTable: "Playlists",
						principalColumn: "Id",
						onDelete: ReferentialAction.Restrict);
					table.ForeignKey(
						name: "FK_PlaylistSong_Songs_SongsId",
						column: x => x.SongsId,
						principalTable: "Songs",
						principalColumn: "Id",
						onDelete: ReferentialAction.Restrict);
				});

			migrationBuilder.CreateIndex(
				name: "IX_Playlists_UserId",
				table: "Playlists",
				column: "UserId");

			migrationBuilder.CreateIndex(
				name: "IX_PlaylistSong_SongsId",
				table: "PlaylistSong",
				column: "SongsId");
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				name: "PlaylistSong");

			migrationBuilder.DropTable(
				name: "Playlists");
		}
	}
}
