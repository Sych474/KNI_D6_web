using Microsoft.EntityFrameworkCore.Migrations;

namespace KNI_D6_web.Model.Migrations
{
    public partial class AddAutoResetToParameter : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AutoReset",
                table: "Parameters",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AutoReset",
                table: "Parameters");
        }
    }
}
