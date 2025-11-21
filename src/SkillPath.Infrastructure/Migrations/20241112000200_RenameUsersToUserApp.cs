using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillPath.Infrastructure.Migrations;

public partial class RenameUsersToUserApp : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Migration vazia - a tabela já é criada como USER_APP na migration anterior
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // Migration vazia - não há nada para reverter
    }
}
