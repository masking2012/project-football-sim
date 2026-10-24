using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectFootballSim.Leagues.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class LeaguesInit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Leagues",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    CountryId = table.Column<int>(type: "int", nullable: false),
                    TeamsCount = table.Column<int>(type: "int", nullable: false),
                    Format = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PromotionPositions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PromotionPlayOffPositions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RelegationPositions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RelegationPlayOffPositions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UefaChampionsLeaguePositions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UefaEuropaLeaguePositions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UefaConferenceLeaguePositions = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Leagues", x => new { x.Id, x.Version });
                });

            migrationBuilder.CreateTable(
                name: "GameLeagues",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GameId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SeasonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LeagueId = table.Column<int>(type: "int", nullable: false),
                    LeagueVersion = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameLeagues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameLeagues_Leagues_LeagueId_LeagueVersion",
                        columns: x => new { x.LeagueId, x.LeagueVersion },
                        principalTable: "Leagues",
                        principalColumns: new[] { "Id", "Version" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LeagueActivations",
                columns: table => new
                {
                    LeagueId = table.Column<int>(type: "int", nullable: false),
                    CurrentVersion = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeagueActivations", x => x.LeagueId);
                    table.ForeignKey(
                        name: "FK_LeagueActivations_Leagues_LeagueId_CurrentVersion",
                        columns: x => new { x.LeagueId, x.CurrentVersion },
                        principalTable: "Leagues",
                        principalColumns: new[] { "Id", "Version" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LeagueRounds",
                columns: table => new
                {
                    LeagueId = table.Column<int>(type: "int", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    Round = table.Column<int>(type: "int", nullable: false),
                    Week = table.Column<int>(type: "int", nullable: false),
                    IsMidweek = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeagueRounds", x => new { x.LeagueId, x.Version, x.Round });
                    table.ForeignKey(
                        name: "FK_LeagueRounds_Leagues_LeagueId_Version",
                        columns: x => new { x.LeagueId, x.Version },
                        principalTable: "Leagues",
                        principalColumns: new[] { "Id", "Version" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LeagueTeams",
                columns: table => new
                {
                    LeagueId = table.Column<int>(type: "int", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    TeamId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeagueTeams", x => new { x.LeagueId, x.Version, x.TeamId });
                    table.ForeignKey(
                        name: "FK_LeagueTeams_Leagues_LeagueId_Version",
                        columns: x => new { x.LeagueId, x.Version },
                        principalTable: "Leagues",
                        principalColumns: new[] { "Id", "Version" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GameLeagueMatches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HomeTeamId = table.Column<int>(type: "int", nullable: false),
                    AwayTeamId = table.Column<int>(type: "int", nullable: false),
                    HomeTeamScore = table.Column<int>(type: "int", nullable: true),
                    AwayTeamScore = table.Column<int>(type: "int", nullable: true),
                    Round = table.Column<int>(type: "int", nullable: false),
                    GameLeagueId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameLeagueMatches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameLeagueMatches_GameLeagues_GameLeagueId",
                        column: x => x.GameLeagueId,
                        principalTable: "GameLeagues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GameLeagueTeams",
                columns: table => new
                {
                    GameLeagueId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TeamId = table.Column<int>(type: "int", nullable: false),
                    Wins = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Draws = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Losses = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    GoalsFor = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    GoalsAgainst = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Points = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameLeagueTeams", x => new { x.GameLeagueId, x.TeamId });
                    table.ForeignKey(
                        name: "FK_GameLeagueTeams_GameLeagues_GameLeagueId",
                        column: x => x.GameLeagueId,
                        principalTable: "GameLeagues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GameLeagueMatches_GameLeagueId",
                table: "GameLeagueMatches",
                column: "GameLeagueId");

            migrationBuilder.CreateIndex(
                name: "IX_GameLeagues_GameId_UserId_SeasonId_LeagueId_LeagueVersion",
                table: "GameLeagues",
                columns: new[] { "GameId", "UserId", "SeasonId", "LeagueId", "LeagueVersion" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GameLeagues_LeagueId_LeagueVersion",
                table: "GameLeagues",
                columns: new[] { "LeagueId", "LeagueVersion" });

            migrationBuilder.CreateIndex(
                name: "IX_LeagueActivations_LeagueId_CurrentVersion",
                table: "LeagueActivations",
                columns: new[] { "LeagueId", "CurrentVersion" });

            migrationBuilder.CreateIndex(
                name: "IX_LeagueRounds_LeagueId_Version_Week_IsMidweek",
                table: "LeagueRounds",
                columns: new[] { "LeagueId", "Version", "Week", "IsMidweek" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameLeagueMatches");

            migrationBuilder.DropTable(
                name: "GameLeagueTeams");

            migrationBuilder.DropTable(
                name: "LeagueActivations");

            migrationBuilder.DropTable(
                name: "LeagueRounds");

            migrationBuilder.DropTable(
                name: "LeagueTeams");

            migrationBuilder.DropTable(
                name: "GameLeagues");

            migrationBuilder.DropTable(
                name: "Leagues");
        }
    }
}
