using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace playerService.Data.Migrations
{
    /// <inheritdoc />
    public partial class StatFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Asists",
                table: "players",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Matchs",
                table: "players",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Score",
                table: "players",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Asists",
                table: "players");

            migrationBuilder.DropColumn(
                name: "Matchs",
                table: "players");

            migrationBuilder.DropColumn(
                name: "Score",
                table: "players");
        }
    }
}
