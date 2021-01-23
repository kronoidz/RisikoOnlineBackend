using Microsoft.EntityFrameworkCore.Migrations;

namespace RisikoOnline.Migrations
{
    public partial class m1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: false),
                    PasswordSalt = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "Invitation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SenderName = table.Column<string>(type: "TEXT", nullable: false),
                    ReceiverName = table.Column<string>(type: "TEXT", nullable: false),
                    Accepted = table.Column<bool>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invitation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Invitation_Players_ReceiverName",
                        column: x => x.ReceiverName,
                        principalTable: "Players",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Invitation_Players_SenderName",
                        column: x => x.SenderName,
                        principalTable: "Players",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MatchHistoryRecord",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PlayerName = table.Column<string>(type: "TEXT", nullable: false),
                    IsWinner = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchHistoryRecord", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MatchHistoryRecord_Players_PlayerName",
                        column: x => x.PlayerName,
                        principalTable: "Players",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerState",
                columns: table => new
                {
                    MatchId = table.Column<int>(type: "INTEGER", nullable: false),
                    PlayerName = table.Column<string>(type: "TEXT", nullable: false),
                    IsInitialized = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    MissionObjective = table.Column<int>(type: "INTEGER", nullable: false),
                    TargetPlayerName = table.Column<string>(type: "TEXT", nullable: true),
                    ReinforcementPoints = table.Column<int>(type: "INTEGER", nullable: false),
                    UnplacedArmies = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerState", x => new { x.MatchId, x.PlayerName });
                    table.ForeignKey(
                        name: "FK_PlayerState_Players_PlayerName",
                        column: x => x.PlayerName,
                        principalTable: "Players",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlayerState_PlayerState_MatchId_TargetPlayerName",
                        columns: x => new { x.MatchId, x.TargetPlayerName },
                        principalTable: "PlayerState",
                        principalColumns: new[] { "MatchId", "PlayerName" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Matches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false),
                    CurrentPlayerName = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Matches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Matches_PlayerState_Id_CurrentPlayerName",
                        columns: x => new { x.Id, x.CurrentPlayerName },
                        principalTable: "PlayerState",
                        principalColumns: new[] { "MatchId", "PlayerName" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TerritoryOwnership",
                columns: table => new
                {
                    MatchId = table.Column<int>(type: "INTEGER", nullable: false),
                    PlayerName = table.Column<string>(type: "TEXT", nullable: false),
                    Territory = table.Column<int>(type: "INTEGER", nullable: false),
                    Armies = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TerritoryOwnership", x => new { x.MatchId, x.PlayerName, x.Territory });
                    table.ForeignKey(
                        name: "FK_TerritoryOwnership_Matches_MatchId",
                        column: x => x.MatchId,
                        principalTable: "Matches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TerritoryOwnership_Players_PlayerName",
                        column: x => x.PlayerName,
                        principalTable: "Players",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TerritoryOwnership_PlayerState_MatchId_PlayerName",
                        columns: x => new { x.MatchId, x.PlayerName },
                        principalTable: "PlayerState",
                        principalColumns: new[] { "MatchId", "PlayerName" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Invitation_ReceiverName",
                table: "Invitation",
                column: "ReceiverName");

            migrationBuilder.CreateIndex(
                name: "IX_Invitation_SenderName",
                table: "Invitation",
                column: "SenderName");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_Id_CurrentPlayerName",
                table: "Matches",
                columns: new[] { "Id", "CurrentPlayerName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MatchHistoryRecord_PlayerName",
                table: "MatchHistoryRecord",
                column: "PlayerName");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerState_MatchId_TargetPlayerName",
                table: "PlayerState",
                columns: new[] { "MatchId", "TargetPlayerName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerState_PlayerName",
                table: "PlayerState",
                column: "PlayerName");

            migrationBuilder.CreateIndex(
                name: "IX_TerritoryOwnership_PlayerName",
                table: "TerritoryOwnership",
                column: "PlayerName");

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerState_Matches_MatchId",
                table: "PlayerState",
                column: "MatchId",
                principalTable: "Matches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlayerState_Players_PlayerName",
                table: "PlayerState");

            migrationBuilder.DropForeignKey(
                name: "FK_Matches_PlayerState_Id_CurrentPlayerName",
                table: "Matches");

            migrationBuilder.DropTable(
                name: "Invitation");

            migrationBuilder.DropTable(
                name: "MatchHistoryRecord");

            migrationBuilder.DropTable(
                name: "TerritoryOwnership");

            migrationBuilder.DropTable(
                name: "Players");

            migrationBuilder.DropTable(
                name: "PlayerState");

            migrationBuilder.DropTable(
                name: "Matches");
        }
    }
}
