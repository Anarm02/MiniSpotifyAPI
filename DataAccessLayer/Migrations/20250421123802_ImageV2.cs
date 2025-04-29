using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class ImageV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Image_ProfileImageId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Playlists_Image_CoverImageId",
                table: "Playlists");

            migrationBuilder.DropForeignKey(
                name: "FK_Songs_Image_ArtworkImageId",
                table: "Songs");

            migrationBuilder.DropTable(
                name: "Image");

            migrationBuilder.CreateTable(
                name: "Images",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageType = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SongId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PlaylistId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Images", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Images_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Images_Playlists_PlaylistId",
                        column: x => x.PlaylistId,
                        principalTable: "Playlists",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Images_Songs_SongId",
                        column: x => x.SongId,
                        principalTable: "Songs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Images_PlaylistId",
                table: "Images",
                column: "PlaylistId");

            migrationBuilder.CreateIndex(
                name: "IX_Images_SongId",
                table: "Images",
                column: "SongId");

            migrationBuilder.CreateIndex(
                name: "IX_Images_UserId",
                table: "Images",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Images_ProfileImageId",
                table: "AspNetUsers",
                column: "ProfileImageId",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Playlists_Images_CoverImageId",
                table: "Playlists",
                column: "CoverImageId",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Songs_Images_ArtworkImageId",
                table: "Songs",
                column: "ArtworkImageId",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Images_ProfileImageId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Playlists_Images_CoverImageId",
                table: "Playlists");

            migrationBuilder.DropForeignKey(
                name: "FK_Songs_Images_ArtworkImageId",
                table: "Songs");

            migrationBuilder.DropTable(
                name: "Images");

            migrationBuilder.CreateTable(
                name: "Image",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ImageType = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Image", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Image_ProfileImageId",
                table: "AspNetUsers",
                column: "ProfileImageId",
                principalTable: "Image",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Playlists_Image_CoverImageId",
                table: "Playlists",
                column: "CoverImageId",
                principalTable: "Image",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Songs_Image_ArtworkImageId",
                table: "Songs",
                column: "ArtworkImageId",
                principalTable: "Image",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
