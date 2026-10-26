using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectFootballSim.Leagues.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class LeagueVersionCatalog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GameLeagues_Leagues_LeagueId_LeagueVersion",
                table: "GameLeagues");

            migrationBuilder.DropForeignKey(
                name: "FK_LeagueActivations_Leagues_LeagueId_CurrentVersion",
                table: "LeagueActivations");

            migrationBuilder.DropForeignKey(
                name: "FK_LeagueRounds_Leagues_LeagueId_Version",
                table: "LeagueRounds");

            migrationBuilder.DropForeignKey(
                name: "FK_LeagueTeams_Leagues_LeagueId_Version",
                table: "LeagueTeams");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LeagueTeams",
                table: "LeagueTeams");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Leagues",
                table: "Leagues");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LeagueRounds",
                table: "LeagueRounds");

            migrationBuilder.DropIndex(
                name: "IX_LeagueRounds_LeagueId_Version_Week_IsMidweek",
                table: "LeagueRounds");

            migrationBuilder.DropIndex(
                name: "IX_LeagueActivations_LeagueId_CurrentVersion",
                table: "LeagueActivations");

            migrationBuilder.DropIndex(
                name: "IX_GameLeagues_LeagueId_LeagueVersion",
                table: "GameLeagues");

            migrationBuilder.DropColumn(
                name: "LeagueId",
                table: "LeagueTeams");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Leagues");

            migrationBuilder.DropColumn(
                name: "Format",
                table: "Leagues");

            migrationBuilder.DropColumn(
                name: "PromotionPlayOffPositions",
                table: "Leagues");

            migrationBuilder.DropColumn(
                name: "PromotionPositions",
                table: "Leagues");

            migrationBuilder.DropColumn(
                name: "RelegationPlayOffPositions",
                table: "Leagues");

            migrationBuilder.DropColumn(
                name: "RelegationPositions",
                table: "Leagues");

            migrationBuilder.DropColumn(
                name: "TeamsCount",
                table: "Leagues");

            migrationBuilder.DropColumn(
                name: "UefaChampionsLeaguePositions",
                table: "Leagues");

            migrationBuilder.DropColumn(
                name: "UefaConferenceLeaguePositions",
                table: "Leagues");

            migrationBuilder.DropColumn(
                name: "UefaEuropaLeaguePositions",
                table: "Leagues");

            migrationBuilder.DropColumn(
                name: "LeagueId",
                table: "LeagueRounds");

            migrationBuilder.RenameColumn(
                name: "Version",
                table: "LeagueTeams",
                newName: "LeagueVersionId");

            migrationBuilder.RenameColumn(
                name: "Version",
                table: "LeagueRounds",
                newName: "LeagueVersionId");

            migrationBuilder.RenameColumn(
                name: "CurrentVersion",
                table: "LeagueActivations",
                newName: "LeagueVersionId");

            migrationBuilder.RenameColumn(
                name: "LeagueVersion",
                table: "GameLeagues",
                newName: "LeagueVersionId");

            migrationBuilder.RenameIndex(
                name: "IX_GameLeagues_GameId_UserId_SeasonId_LeagueId_LeagueVersion",
                table: "GameLeagues",
                newName: "IX_GameLeagues_GameId_UserId_SeasonId_LeagueId_LeagueVersionId");

            migrationBuilder.AlterColumn<int>(
                name: "LeagueId",
                table: "LeagueActivations",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LeagueTeams",
                table: "LeagueTeams",
                columns: new[] { "LeagueVersionId", "TeamId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Leagues",
                table: "Leagues",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LeagueRounds",
                table: "LeagueRounds",
                columns: new[] { "LeagueVersionId", "Round" });

            migrationBuilder.CreateTable(
                name: "LeagueVersions",
                columns: table => new
                {
                    LeagueVersionId = table.Column<int>(type: "int", nullable: false),
                    LeagueId = table.Column<int>(type: "int", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_LeagueVersions", x => x.LeagueVersionId);
                    table.ForeignKey(
                        name: "FK_LeagueVersions_Leagues_LeagueId",
                        column: x => x.LeagueId,
                        principalTable: "Leagues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LeagueRounds_LeagueVersionId_Week_IsMidweek",
                table: "LeagueRounds",
                columns: new[] { "LeagueVersionId", "Week", "IsMidweek" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LeagueActivations_LeagueVersionId",
                table: "LeagueActivations",
                column: "LeagueVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_GameLeagues_LeagueVersionId",
                table: "GameLeagues",
                column: "LeagueVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_LeagueVersions_LeagueId_Version",
                table: "LeagueVersions",
                columns: new[] { "LeagueId", "Version" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_GameLeagues_LeagueVersions_LeagueVersionId",
                table: "GameLeagues",
                column: "LeagueVersionId",
                principalTable: "LeagueVersions",
                principalColumn: "LeagueVersionId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LeagueActivations_LeagueVersions_LeagueVersionId",
                table: "LeagueActivations",
                column: "LeagueVersionId",
                principalTable: "LeagueVersions",
                principalColumn: "LeagueVersionId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LeagueRounds_LeagueVersions_LeagueVersionId",
                table: "LeagueRounds",
                column: "LeagueVersionId",
                principalTable: "LeagueVersions",
                principalColumn: "LeagueVersionId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LeagueTeams_LeagueVersions_LeagueVersionId",
                table: "LeagueTeams",
                column: "LeagueVersionId",
                principalTable: "LeagueVersions",
                principalColumn: "LeagueVersionId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GameLeagues_LeagueVersions_LeagueVersionId",
                table: "GameLeagues");

            migrationBuilder.DropForeignKey(
                name: "FK_LeagueActivations_LeagueVersions_LeagueVersionId",
                table: "LeagueActivations");

            migrationBuilder.DropForeignKey(
                name: "FK_LeagueRounds_LeagueVersions_LeagueVersionId",
                table: "LeagueRounds");

            migrationBuilder.DropForeignKey(
                name: "FK_LeagueTeams_LeagueVersions_LeagueVersionId",
                table: "LeagueTeams");

            migrationBuilder.DropTable(
                name: "LeagueVersions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LeagueTeams",
                table: "LeagueTeams");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Leagues",
                table: "Leagues");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LeagueRounds",
                table: "LeagueRounds");

            migrationBuilder.DropIndex(
                name: "IX_LeagueRounds_LeagueVersionId_Week_IsMidweek",
                table: "LeagueRounds");

            migrationBuilder.DropIndex(
                name: "IX_LeagueActivations_LeagueVersionId",
                table: "LeagueActivations");

            migrationBuilder.DropIndex(
                name: "IX_GameLeagues_LeagueVersionId",
                table: "GameLeagues");

            migrationBuilder.RenameColumn(
                name: "LeagueVersionId",
                table: "LeagueTeams",
                newName: "Version");

            migrationBuilder.RenameColumn(
                name: "LeagueVersionId",
                table: "LeagueRounds",
                newName: "Version");

            migrationBuilder.RenameColumn(
                name: "LeagueVersionId",
                table: "LeagueActivations",
                newName: "CurrentVersion");

            migrationBuilder.RenameColumn(
                name: "LeagueVersionId",
                table: "GameLeagues",
                newName: "LeagueVersion");

            migrationBuilder.RenameIndex(
                name: "IX_GameLeagues_GameId_UserId_SeasonId_LeagueId_LeagueVersionId",
                table: "GameLeagues",
                newName: "IX_GameLeagues_GameId_UserId_SeasonId_LeagueId_LeagueVersion");

            migrationBuilder.AddColumn<int>(
                name: "LeagueId",
                table: "LeagueTeams",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "Leagues",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Format",
                table: "Leagues",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PromotionPlayOffPositions",
                table: "Leagues",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PromotionPositions",
                table: "Leagues",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RelegationPlayOffPositions",
                table: "Leagues",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RelegationPositions",
                table: "Leagues",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TeamsCount",
                table: "Leagues",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "UefaChampionsLeaguePositions",
                table: "Leagues",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UefaConferenceLeaguePositions",
                table: "Leagues",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UefaEuropaLeaguePositions",
                table: "Leagues",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LeagueId",
                table: "LeagueRounds",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "LeagueId",
                table: "LeagueActivations",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LeagueTeams",
                table: "LeagueTeams",
                columns: new[] { "LeagueId", "Version", "TeamId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Leagues",
                table: "Leagues",
                columns: new[] { "Id", "Version" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_LeagueRounds",
                table: "LeagueRounds",
                columns: new[] { "LeagueId", "Version", "Round" });

            migrationBuilder.CreateIndex(
                name: "IX_LeagueRounds_LeagueId_Version_Week_IsMidweek",
                table: "LeagueRounds",
                columns: new[] { "LeagueId", "Version", "Week", "IsMidweek" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LeagueActivations_LeagueId_CurrentVersion",
                table: "LeagueActivations",
                columns: new[] { "LeagueId", "CurrentVersion" });

            migrationBuilder.CreateIndex(
                name: "IX_GameLeagues_LeagueId_LeagueVersion",
                table: "GameLeagues",
                columns: new[] { "LeagueId", "LeagueVersion" });

            migrationBuilder.AddForeignKey(
                name: "FK_GameLeagues_Leagues_LeagueId_LeagueVersion",
                table: "GameLeagues",
                columns: new[] { "LeagueId", "LeagueVersion" },
                principalTable: "Leagues",
                principalColumns: new[] { "Id", "Version" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LeagueActivations_Leagues_LeagueId_CurrentVersion",
                table: "LeagueActivations",
                columns: new[] { "LeagueId", "CurrentVersion" },
                principalTable: "Leagues",
                principalColumns: new[] { "Id", "Version" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LeagueRounds_Leagues_LeagueId_Version",
                table: "LeagueRounds",
                columns: new[] { "LeagueId", "Version" },
                principalTable: "Leagues",
                principalColumns: new[] { "Id", "Version" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LeagueTeams_Leagues_LeagueId_Version",
                table: "LeagueTeams",
                columns: new[] { "LeagueId", "Version" },
                principalTable: "Leagues",
                principalColumns: new[] { "Id", "Version" },
                onDelete: ReferentialAction.Cascade);
        }
    }
}
