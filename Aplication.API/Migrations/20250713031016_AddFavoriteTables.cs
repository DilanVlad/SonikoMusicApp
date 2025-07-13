using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Application.API.Migrations
{
    /// <inheritdoc />
    public partial class AddFavoriteTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FavoriteArtist_AspNetUsers_ArtistId",
                table: "FavoriteArtist");

            migrationBuilder.DropForeignKey(
                name: "FK_FavoriteArtist_AspNetUsers_UserId",
                table: "FavoriteArtist");

            migrationBuilder.DropForeignKey(
                name: "FK_FavoriteMusic_AspNetUsers_UserId",
                table: "FavoriteMusic");

            migrationBuilder.DropForeignKey(
                name: "FK_FavoriteMusic_Musics_MusicId",
                table: "FavoriteMusic");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FavoriteMusic",
                table: "FavoriteMusic");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FavoriteArtist",
                table: "FavoriteArtist");

            migrationBuilder.RenameTable(
                name: "FavoriteMusic",
                newName: "FavoriteMusics");

            migrationBuilder.RenameTable(
                name: "FavoriteArtist",
                newName: "FavoriteArtists");

            migrationBuilder.RenameIndex(
                name: "IX_FavoriteMusic_UserId",
                table: "FavoriteMusics",
                newName: "IX_FavoriteMusics_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_FavoriteMusic_MusicId",
                table: "FavoriteMusics",
                newName: "IX_FavoriteMusics_MusicId");

            migrationBuilder.RenameIndex(
                name: "IX_FavoriteArtist_UserId",
                table: "FavoriteArtists",
                newName: "IX_FavoriteArtists_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_FavoriteArtist_ArtistId",
                table: "FavoriteArtists",
                newName: "IX_FavoriteArtists_ArtistId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FavoriteMusics",
                table: "FavoriteMusics",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FavoriteArtists",
                table: "FavoriteArtists",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FavoriteArtists_AspNetUsers_ArtistId",
                table: "FavoriteArtists",
                column: "ArtistId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FavoriteArtists_AspNetUsers_UserId",
                table: "FavoriteArtists",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FavoriteMusics_AspNetUsers_UserId",
                table: "FavoriteMusics",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_FavoriteMusics_Musics_MusicId",
                table: "FavoriteMusics",
                column: "MusicId",
                principalTable: "Musics",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FavoriteArtists_AspNetUsers_ArtistId",
                table: "FavoriteArtists");

            migrationBuilder.DropForeignKey(
                name: "FK_FavoriteArtists_AspNetUsers_UserId",
                table: "FavoriteArtists");

            migrationBuilder.DropForeignKey(
                name: "FK_FavoriteMusics_AspNetUsers_UserId",
                table: "FavoriteMusics");

            migrationBuilder.DropForeignKey(
                name: "FK_FavoriteMusics_Musics_MusicId",
                table: "FavoriteMusics");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FavoriteMusics",
                table: "FavoriteMusics");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FavoriteArtists",
                table: "FavoriteArtists");

            migrationBuilder.RenameTable(
                name: "FavoriteMusics",
                newName: "FavoriteMusic");

            migrationBuilder.RenameTable(
                name: "FavoriteArtists",
                newName: "FavoriteArtist");

            migrationBuilder.RenameIndex(
                name: "IX_FavoriteMusics_UserId",
                table: "FavoriteMusic",
                newName: "IX_FavoriteMusic_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_FavoriteMusics_MusicId",
                table: "FavoriteMusic",
                newName: "IX_FavoriteMusic_MusicId");

            migrationBuilder.RenameIndex(
                name: "IX_FavoriteArtists_UserId",
                table: "FavoriteArtist",
                newName: "IX_FavoriteArtist_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_FavoriteArtists_ArtistId",
                table: "FavoriteArtist",
                newName: "IX_FavoriteArtist_ArtistId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FavoriteMusic",
                table: "FavoriteMusic",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FavoriteArtist",
                table: "FavoriteArtist",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FavoriteArtist_AspNetUsers_ArtistId",
                table: "FavoriteArtist",
                column: "ArtistId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FavoriteArtist_AspNetUsers_UserId",
                table: "FavoriteArtist",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FavoriteMusic_AspNetUsers_UserId",
                table: "FavoriteMusic",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_FavoriteMusic_Musics_MusicId",
                table: "FavoriteMusic",
                column: "MusicId",
                principalTable: "Musics",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }
    }
}
