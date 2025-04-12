using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace playerService.Data.Migrations
{
    /// <inheritdoc />
    public partial class FieldModification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Score",
                table: "players",
                newName: "Scores");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Scores",
                table: "players",
                newName: "Score");
        }
    }
}
