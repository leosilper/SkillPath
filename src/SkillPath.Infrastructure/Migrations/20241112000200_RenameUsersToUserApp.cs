using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillPath.Infrastructure.Migrations;

public partial class RenameUsersToUserApp : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Plans_Users_UserId",
            table: "Plans");

        migrationBuilder.RenameTable(
            name: "Users",
            newName: "USER_APP",
            schema: null);

        migrationBuilder.RenameIndex(
            name: "IX_Users_Email",
            table: "USER_APP",
            newName: "IX_USER_APP_Email");

        migrationBuilder.AddForeignKey(
            name: "FK_Plans_USER_APP_UserId",
            table: "Plans",
            column: "UserId",
            principalTable: "USER_APP",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Plans_USER_APP_UserId",
            table: "Plans");

        migrationBuilder.RenameIndex(
            name: "IX_USER_APP_Email",
            table: "USER_APP",
            newName: "IX_Users_Email");

        migrationBuilder.RenameTable(
            name: "USER_APP",
            newName: "Users",
            schema: null);

        migrationBuilder.AddForeignKey(
            name: "FK_Plans_Users_UserId",
            table: "Plans",
            column: "UserId",
            principalTable: "Users",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);
    }
}
