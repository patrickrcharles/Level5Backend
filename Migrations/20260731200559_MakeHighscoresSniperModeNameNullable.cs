using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Level5Backend.Migrations
{
    /// <inheritdoc />
    public partial class MakeHighscoresSniperModeNameNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "sniperModeName",
                table: "highscores",
                type: "character varying(45)",
                maxLength: 45,
                nullable: true,
                defaultValueSql: "'none'",
                oldClrType: typeof(string),
                oldType: "character varying(45)",
                oldMaxLength: 45,
                oldDefaultValueSql: "'none'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "sniperModeName",
                table: "highscores",
                type: "character varying(45)",
                maxLength: 45,
                nullable: false,
                defaultValueSql: "'none'",
                oldClrType: typeof(string),
                oldType: "character varying(45)",
                oldMaxLength: 45,
                oldNullable: true,
                oldDefaultValueSql: "'none'");
        }
    }
}
