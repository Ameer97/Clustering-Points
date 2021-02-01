using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

namespace Clustering.Migrations
{
    public partial class vity_geom : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TextFormat",
                table: "Cities",
                nullable: true);

            migrationBuilder.AddColumn<Geometry>(
                name: "Geom",
                table: "Cities",
                nullable: true,
                computedColumnSql: "ST_GeomFromText(\"TextFormat\",4326)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Geom",
                table: "Cities");

            migrationBuilder.DropColumn(
                name: "TextFormat",
                table: "Cities");
        }
    }
}
