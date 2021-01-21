using Microsoft.EntityFrameworkCore.Migrations;

namespace RisikoOnline.Migrations
{
    public partial class m4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invitation_Players_ReceiverName",
                table: "Invitation");

            migrationBuilder.DropForeignKey(
                name: "FK_Invitation_Players_SenderName",
                table: "Invitation");

            migrationBuilder.AlterColumn<string>(
                name: "SenderName",
                table: "Invitation",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "ReceiverName",
                table: "Invitation",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddForeignKey(
                name: "FK_Invitation_Players_ReceiverName",
                table: "Invitation",
                column: "ReceiverName",
                principalTable: "Players",
                principalColumn: "Name",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Invitation_Players_SenderName",
                table: "Invitation",
                column: "SenderName",
                principalTable: "Players",
                principalColumn: "Name",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invitation_Players_ReceiverName",
                table: "Invitation");

            migrationBuilder.DropForeignKey(
                name: "FK_Invitation_Players_SenderName",
                table: "Invitation");

            migrationBuilder.AlterColumn<string>(
                name: "SenderName",
                table: "Invitation",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ReceiverName",
                table: "Invitation",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Invitation_Players_ReceiverName",
                table: "Invitation",
                column: "ReceiverName",
                principalTable: "Players",
                principalColumn: "Name",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Invitation_Players_SenderName",
                table: "Invitation",
                column: "SenderName",
                principalTable: "Players",
                principalColumn: "Name",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
