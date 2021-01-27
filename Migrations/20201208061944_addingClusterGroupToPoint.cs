using Microsoft.EntityFrameworkCore.Migrations;

namespace Clustering.Migrations
{
    public partial class addingClusterGroupToPoint : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ClusterGroup",
                table: "Points",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClusterGroup",
                table: "Points");
        }
    }
}
