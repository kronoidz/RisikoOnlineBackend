using Microsoft.EntityFrameworkCore.Migrations;

namespace RisikoOnline.Migrations
{
    public partial class m4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Matches_PlayerState_Id_CurrentPlayerName",
                table: "Matches");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerState_Matches_MatchId",
                table: "PlayerState");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerState_Players_PlayerName",
                table: "PlayerState");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerState_PlayerState_MatchId_TargetPlayerName",
                table: "PlayerState");

            migrationBuilder.DropForeignKey(
                name: "FK_TerritoryOwnership_PlayerState_MatchId_PlayerName",
                table: "TerritoryOwnership");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlayerState",
                table: "PlayerState");

            migrationBuilder.RenameTable(
                name: "PlayerState",
                newName: "PlayerStates");

            migrationBuilder.RenameIndex(
                name: "IX_PlayerState_PlayerName",
                table: "PlayerStates",
                newName: "IX_PlayerStates_PlayerName");

            migrationBuilder.RenameIndex(
                name: "IX_PlayerState_MatchId_TargetPlayerName",
                table: "PlayerStates",
                newName: "IX_PlayerStates_MatchId_TargetPlayerName");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlayerStates",
                table: "PlayerStates",
                columns: new[] { "MatchId", "PlayerName" });

            migrationBuilder.AddForeignKey(
                name: "FK_Matches_PlayerStates_Id_CurrentPlayerName",
                table: "Matches",
                columns: new[] { "Id", "CurrentPlayerName" },
                principalTable: "PlayerStates",
                principalColumns: new[] { "MatchId", "PlayerName" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerStates_Matches_MatchId",
                table: "PlayerStates",
                column: "MatchId",
                principalTable: "Matches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerStates_Players_PlayerName",
                table: "PlayerStates",
                column: "PlayerName",
                principalTable: "Players",
                principalColumn: "Name",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerStates_PlayerStates_MatchId_TargetPlayerName",
                table: "PlayerStates",
                columns: new[] { "MatchId", "TargetPlayerName" },
                principalTable: "PlayerStates",
                principalColumns: new[] { "MatchId", "PlayerName" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TerritoryOwnership_PlayerStates_MatchId_PlayerName",
                table: "TerritoryOwnership",
                columns: new[] { "MatchId", "PlayerName" },
                principalTable: "PlayerStates",
                principalColumns: new[] { "MatchId", "PlayerName" },
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Matches_PlayerStates_Id_CurrentPlayerName",
                table: "Matches");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerStates_Matches_MatchId",
                table: "PlayerStates");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerStates_Players_PlayerName",
                table: "PlayerStates");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerStates_PlayerStates_MatchId_TargetPlayerName",
                table: "PlayerStates");

            migrationBuilder.DropForeignKey(
                name: "FK_TerritoryOwnership_PlayerStates_MatchId_PlayerName",
                table: "TerritoryOwnership");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlayerStates",
                table: "PlayerStates");

            migrationBuilder.RenameTable(
                name: "PlayerStates",
                newName: "PlayerState");

            migrationBuilder.RenameIndex(
                name: "IX_PlayerStates_PlayerName",
                table: "PlayerState",
                newName: "IX_PlayerState_PlayerName");

            migrationBuilder.RenameIndex(
                name: "IX_PlayerStates_MatchId_TargetPlayerName",
                table: "PlayerState",
                newName: "IX_PlayerState_MatchId_TargetPlayerName");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlayerState",
                table: "PlayerState",
                columns: new[] { "MatchId", "PlayerName" });

            migrationBuilder.AddForeignKey(
                name: "FK_Matches_PlayerState_Id_CurrentPlayerName",
                table: "Matches",
                columns: new[] { "Id", "CurrentPlayerName" },
                principalTable: "PlayerState",
                principalColumns: new[] { "MatchId", "PlayerName" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerState_Matches_MatchId",
                table: "PlayerState",
                column: "MatchId",
                principalTable: "Matches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerState_Players_PlayerName",
                table: "PlayerState",
                column: "PlayerName",
                principalTable: "Players",
                principalColumn: "Name",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerState_PlayerState_MatchId_TargetPlayerName",
                table: "PlayerState",
                columns: new[] { "MatchId", "TargetPlayerName" },
                principalTable: "PlayerState",
                principalColumns: new[] { "MatchId", "PlayerName" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TerritoryOwnership_PlayerState_MatchId_PlayerName",
                table: "TerritoryOwnership",
                columns: new[] { "MatchId", "PlayerName" },
                principalTable: "PlayerState",
                principalColumns: new[] { "MatchId", "PlayerName" },
                onDelete: ReferentialAction.Cascade);
        }
    }
}
