using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VtorProektRSWEB.Data.Migrations
{
    /// <inheritdoc />
    public partial class DownloadTrackName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserPlaylists_AspNetUsers_AppUserId",
                table: "UserPlaylists");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPlaylists_Track_TrackId",
                table: "UserPlaylists");

            migrationBuilder.AlterColumn<Guid>(
                name: "TrackId",
                table: "UserPlaylists",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AppUserId",
                table: "UserPlaylists",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ArtistName",
                table: "UserPlaylists",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TrackName",
                table: "UserPlaylists",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DownloadURL",
                table: "Album",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPlaylists_AspNetUsers_AppUserId",
                table: "UserPlaylists",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPlaylists_Track_TrackId",
                table: "UserPlaylists",
                column: "TrackId",
                principalTable: "Track",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserPlaylists_AspNetUsers_AppUserId",
                table: "UserPlaylists");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPlaylists_Track_TrackId",
                table: "UserPlaylists");

            migrationBuilder.DropColumn(
                name: "ArtistName",
                table: "UserPlaylists");

            migrationBuilder.DropColumn(
                name: "TrackName",
                table: "UserPlaylists");

            migrationBuilder.DropColumn(
                name: "DownloadURL",
                table: "Album");

            migrationBuilder.AlterColumn<Guid>(
                name: "TrackId",
                table: "UserPlaylists",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "AppUserId",
                table: "UserPlaylists",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_UserPlaylists_AspNetUsers_AppUserId",
                table: "UserPlaylists",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserPlaylists_Track_TrackId",
                table: "UserPlaylists",
                column: "TrackId",
                principalTable: "Track",
                principalColumn: "Id");
        }
    }
}
