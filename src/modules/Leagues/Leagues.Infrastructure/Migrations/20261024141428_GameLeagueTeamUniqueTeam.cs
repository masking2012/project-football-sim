using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectFootballSim.Leagues.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class GameLeagueTeamUniqueTeam : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_GameLeagueTeams_GameLeagueId",
                table: "GameLeagueTeams");

            migrationBuilder.CreateIndex(
                name: "IX_GameLeagueTeams_GameLeagueId_TeamId",
                table: "GameLeagueTeams",
                columns: new[] { "GameLeagueId", "TeamId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_GameLeagueTeams_GameLeagueId_TeamId",
                table: "GameLeagueTeams");

            migrationBuilder.CreateIndex(
                name: "IX_GameLeagueTeams_GameLeagueId",
                table: "GameLeagueTeams",
                column: "GameLeagueId");
        }
    }
}
