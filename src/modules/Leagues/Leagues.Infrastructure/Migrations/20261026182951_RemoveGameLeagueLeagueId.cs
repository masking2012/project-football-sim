using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectFootballSim.Leagues.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveGameLeagueLeagueId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_GameLeagues_GameId_UserId_SeasonId_LeagueId_LeagueVersionId",
                table: "GameLeagues");

            migrationBuilder.DropColumn(
                name: "LeagueId",
                table: "GameLeagues");

            migrationBuilder.CreateIndex(
                name: "IX_GameLeagues_GameId_UserId_SeasonId_LeagueVersionId",
                table: "GameLeagues",
                columns: new[] { "GameId", "UserId", "SeasonId", "LeagueVersionId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_GameLeagues_GameId_UserId_SeasonId_LeagueVersionId",
                table: "GameLeagues");

            migrationBuilder.AddColumn<int>(
                name: "LeagueId",
                table: "GameLeagues",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_GameLeagues_GameId_UserId_SeasonId_LeagueId_LeagueVersionId",
                table: "GameLeagues",
                columns: new[] { "GameId", "UserId", "SeasonId", "LeagueId", "LeagueVersionId" },
                unique: true);
        }
    }
}
