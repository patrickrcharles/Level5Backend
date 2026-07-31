using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Level5Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddHighscoresModeidUseridIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_highscores_modeid_userid",
                table: "highscores",
                columns: new[] { "modeid", "userid" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_highscores_modeid_userid",
                table: "highscores");
        }
    }
}
