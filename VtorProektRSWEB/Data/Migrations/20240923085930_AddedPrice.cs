using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VtorProektRSWEB.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedPrice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "priceVinyl",
                table: "Album",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "priceVinyl",
                table: "Album");
        }
    }
}
