using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Clustering.Migrations
{
    public partial class add_city_table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "City",
                table: "Points");

            migrationBuilder.AddColumn<int>(
                name: "CityId",
                table: "Points",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Cities",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cities", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Points_CityId",
                table: "Points",
                column: "CityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Points_Cities_CityId",
                table: "Points",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Points_Cities_CityId",
                table: "Points");

            migrationBuilder.DropTable(
                name: "Cities");

            migrationBuilder.DropIndex(
                name: "IX_Points_CityId",
                table: "Points");

            migrationBuilder.DropColumn(
                name: "CityId",
                table: "Points");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Points",
                type: "text",
                nullable: true);
        }
    }
}
