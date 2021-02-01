using Microsoft.EntityFrameworkCore.Migrations;

namespace Clustering.Migrations
{
    public partial class city_poly : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TextPoly",
                table: "Cities",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TextPoly",
                table: "Cities");
        }
    }
}
