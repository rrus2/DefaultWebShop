using Microsoft.EntityFrameworkCore.Migrations;

namespace DefaultWebShop.Migrations
{
    public partial class SeedRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("INSERT INTO AspNetRoles (Id, Name) VALUES (1, 'Admin')");
            migrationBuilder.Sql("INSERT INTO AspNetRoles (Id, Name) VALUES (2, 'User')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM AspNetRoles");
        }
    }
}
