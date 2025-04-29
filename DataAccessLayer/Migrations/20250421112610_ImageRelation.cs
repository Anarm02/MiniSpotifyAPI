using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class ImageRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ArtworkImageId",
                table: "Songs",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CoverImageId",
                table: "Playlists",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ProfileImageId",
                table: "AspNetUsers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Image",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageType = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Image", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Songs_ArtworkImageId",
                table: "Songs",
                column: "ArtworkImageId",
                unique: true,
                filter: "[ArtworkImageId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Playlists_CoverImageId",
                table: "Playlists",
                column: "CoverImageId",
                unique: true,
                filter: "[CoverImageId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_ProfileImageId",
                table: "AspNetUsers",
                column: "ProfileImageId",
                unique: true,
                filter: "[ProfileImageId] IS NOT NULL");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropIndex(
                name: "IX_Songs_ArtworkImageId",
                table: "Songs");

            migrationBuilder.DropIndex(
                name: "IX_Playlists_CoverImageId",
                table: "Playlists");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_ProfileImageId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ArtworkImageId",
                table: "Songs");

            migrationBuilder.DropColumn(
                name: "CoverImageId",
                table: "Playlists");

            migrationBuilder.DropColumn(
                name: "ProfileImageId",
                table: "AspNetUsers");
        }
    }
}
