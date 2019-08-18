using Microsoft.EntityFrameworkCore.Migrations;

namespace KNI_D6_web.Model.Migrations
{
    public partial class AddDescriptionToAchievement : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Achievements",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Achievements");
        }
    }
}
