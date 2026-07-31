using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Level5Backend.Migrations
{
    /// <inheritdoc />
    public partial class MakeHighscoresDifficultyNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "difficulty",
                table: "highscores",
                type: "integer",
                nullable: true,
                defaultValueSql: "'1'",
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValueSql: "'1'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "difficulty",
                table: "highscores",
                type: "integer",
                nullable: false,
                defaultValueSql: "'1'",
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true,
                oldDefaultValueSql: "'1'");
        }
    }
}
