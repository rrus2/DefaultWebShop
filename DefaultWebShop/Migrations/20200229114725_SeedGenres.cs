using Microsoft.EntityFrameworkCore.Migrations;

namespace DefaultWebShop.Migrations
{
    public partial class SeedGenres : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("INSERT INTO Genres (Name) VALUES ('Weapon')");
            migrationBuilder.Sql("INSERT INTO Genres (Name) VALUES ('Vehicle')");
            migrationBuilder.Sql("INSERT INTO Genres (Name) VALUES ('Book')");
            migrationBuilder.Sql("INSERT INTO Genres (Name) VALUES ('Other')");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM Genres");
        }
    }
}
