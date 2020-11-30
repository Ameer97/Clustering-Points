using Microsoft.EntityFrameworkCore.Migrations;

namespace Clustering.Migrations
{
    public partial class remove_city_from_point : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Points_Cities_CityId",
                table: "Points");

            migrationBuilder.DropIndex(
                name: "IX_Points_CityId",
                table: "Points");

            migrationBuilder.DropColumn(
                name: "CityId",
                table: "Points");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CityId",
                table: "Points",
                type: "integer",
                nullable: false,
                defaultValue: 0);

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
    }
}
