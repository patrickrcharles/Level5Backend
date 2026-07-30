using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Level5Backend.Migrations
{
    /// <inheritdoc />
    public partial class FixServerStatKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ServerStats_id",
                table: "ServerStats");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ServerStats",
                table: "ServerStats",
                column: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ServerStats",
                table: "ServerStats");

            migrationBuilder.CreateIndex(
                name: "IX_ServerStats_id",
                table: "ServerStats",
                column: "id",
                unique: true);
        }
    }
}
