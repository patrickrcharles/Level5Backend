using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Level5Backend.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Application",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    currentVersion = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Application", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "highscores",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    userid = table.Column<int>(type: "integer", nullable: false),
                    username = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    scoreid = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true, comment: "unique"),
                    modeid = table.Column<int>(type: "integer", nullable: false),
                    modeName = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    characterid = table.Column<int>(type: "integer", nullable: false),
                    levelid = table.Column<int>(type: "integer", nullable: false),
                    character = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false),
                    level = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false),
                    os = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false),
                    version = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false),
                    date = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false),
                    difficulty = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "'1'"),
                    time = table.Column<float>(type: "real", nullable: false),
                    totalPoints = table.Column<int>(type: "integer", nullable: false),
                    longestShot = table.Column<float>(type: "real", nullable: false),
                    totalDistance = table.Column<float>(type: "real", nullable: false),
                    consecutiveShots = table.Column<int>(type: "integer", nullable: false),
                    trafficEnabled = table.Column<int>(type: "integer", nullable: false),
                    hardcoreEnabled = table.Column<int>(type: "integer", nullable: false),
                    enemiesEnabled = table.Column<int>(type: "integer", nullable: false),
                    enemiesKilled = table.Column<int>(type: "integer", nullable: false),
                    sniperEnabled = table.Column<int>(type: "integer", nullable: false),
                    sniperMode = table.Column<int>(type: "integer", nullable: false),
                    sniperModeName = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false, defaultValueSql: "'none'"),
                    sniperHits = table.Column<int>(type: "integer", nullable: false),
                    sniperShots = table.Column<int>(type: "integer", nullable: false),
                    maxShotMade = table.Column<int>(type: "integer", nullable: false),
                    maxShotAtt = table.Column<int>(type: "integer", nullable: false),
                    twoMade = table.Column<int>(type: "integer", nullable: true, defaultValueSql: "'0'"),
                    twoAtt = table.Column<int>(type: "integer", nullable: true),
                    threeMade = table.Column<int>(type: "integer", nullable: true),
                    threeAtt = table.Column<int>(type: "integer", nullable: true),
                    fourMade = table.Column<int>(type: "integer", nullable: true),
                    fourAtt = table.Column<int>(type: "integer", nullable: true),
                    sevenMade = table.Column<int>(type: "integer", nullable: true),
                    sevenAtt = table.Column<int>(type: "integer", nullable: true),
                    bonusPoints = table.Column<int>(type: "integer", nullable: true),
                    moneyBallMade = table.Column<int>(type: "integer", nullable: true),
                    moneyBallAtt = table.Column<int>(type: "integer", nullable: true),
                    platform = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true, comment: "if desktop/mobile"),
                    device = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true, comment: "what specific device being used"),
                    ipaddress = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    p1TotalPoints = table.Column<int>(type: "integer", nullable: false),
                    p2TotalPoints = table.Column<int>(type: "integer", nullable: false),
                    p3TotalPoints = table.Column<int>(type: "integer", nullable: false),
                    p4TotalPoints = table.Column<int>(type: "integer", nullable: false),
                    firstPlace = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    secondPlace = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    thirdPlace = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    fourthPlace = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    p1IsCpu = table.Column<int>(type: "integer", nullable: false),
                    p2IsCpu = table.Column<int>(type: "integer", nullable: false),
                    p3IsCpu = table.Column<int>(type: "integer", nullable: false),
                    p4IsCpu = table.Column<int>(type: "integer", nullable: false),
                    numPlayers = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_highscores", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ServerMessages",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    message = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    date = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServerMessages", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ServerStats",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    numberOfUsers = table.Column<int>(type: "integer", nullable: true),
                    NumberOfTotalTimePlayed = table.Column<float>(type: "real", nullable: true),
                    NumberOfGamesPlayed = table.Column<int>(type: "integer", nullable: true),
                    NumberOfGamesPlayedHardcore = table.Column<int>(type: "integer", nullable: true),
                    NumberofGamesPlayedTraffic = table.Column<int>(type: "integer", nullable: true),
                    NumberofGamesPlayedEnemies = table.Column<int>(type: "integer", nullable: true),
                    NumberofGamesPlayedSniper = table.Column<int>(type: "integer", nullable: true),
                    NumberOfTotal2ShotsMade = table.Column<int>(type: "integer", nullable: true),
                    NumberOfTotal2ShotsAtt = table.Column<int>(type: "integer", nullable: true),
                    NumberOfTotal3ShotsMade = table.Column<int>(type: "integer", nullable: true),
                    NumberOfTotal3ShotsAtt = table.Column<int>(type: "integer", nullable: true),
                    NumberOfTotal4ShotsMade = table.Column<int>(type: "integer", nullable: true),
                    NumberOfTotal4ShotsAtt = table.Column<int>(type: "integer", nullable: true),
                    NumberOfTotal7ShotsMade = table.Column<int>(type: "integer", nullable: true),
                    NumberOfTotal7ShotsAtt = table.Column<int>(type: "integer", nullable: true),
                    NumberOfTotalMoneyShotsMade = table.Column<int>(type: "integer", nullable: true),
                    NumberOfTotalMoneyShotsAtt = table.Column<int>(type: "integer", nullable: true),
                    NumberOfTotalTotalShotsMade = table.Column<int>(type: "integer", nullable: true),
                    NumberOfTotalTotalShotsAtt = table.Column<int>(type: "integer", nullable: true),
                    NumberOfTotalTotalPointsScored = table.Column<int>(type: "integer", nullable: true),
                    NumberOfTotalEnemiesKilled = table.Column<int>(type: "integer", nullable: true),
                    MostPlayedCharacter = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    MostPlayedLevel = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    MostConsecutiveShots = table.Column<int>(type: "integer", nullable: true),
                    MostConsecutiveShotsUsername = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    LongestShot = table.Column<float>(type: "real", nullable: true),
                    LongestShotUsername = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "UserReport",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    report = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    userid = table.Column<int>(type: "integer", nullable: false),
                    userName = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false),
                    os = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false),
                    device = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false),
                    deviceName = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false),
                    version = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false),
                    ipaddress = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false),
                    date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserReport", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    userid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    username = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false),
                    firstname = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    lastname = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    password = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false),
                    email = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false),
                    ipaddress = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    signupdate = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    lastlogin = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    isdev = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.userid);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Application_currentVersion",
                table: "Application",
                column: "currentVersion",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_highscores_scoreid",
                table: "highscores",
                column: "scoreid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServerStats_id",
                table: "ServerStats",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserReport_id",
                table: "UserReport",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_userid",
                table: "users",
                column: "userid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_username",
                table: "users",
                column: "username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Application");

            migrationBuilder.DropTable(
                name: "highscores");

            migrationBuilder.DropTable(
                name: "ServerMessages");

            migrationBuilder.DropTable(
                name: "ServerStats");

            migrationBuilder.DropTable(
                name: "UserReport");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
