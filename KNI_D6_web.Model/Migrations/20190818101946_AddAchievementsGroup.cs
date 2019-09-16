using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace KNI_D6_web.Model.Migrations
{
    public partial class AddAchievementsGroup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NumberInGroup",
                table: "Achievements",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "AchievementGroups",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AchievementGroups", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Achievements_AchievementsGroupId",
                table: "Achievements",
                column: "AchievementsGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Achievements_AchievementGroups_AchievementsGroupId",
                table: "Achievements",
                column: "AchievementsGroupId",
                principalTable: "AchievementGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Achievements_AchievementGroups_AchievementsGroupId",
                table: "Achievements");

            migrationBuilder.DropTable(
                name: "AchievementGroups");

            migrationBuilder.DropIndex(
                name: "IX_Achievements_AchievementsGroupId",
                table: "Achievements");

            migrationBuilder.DropColumn(
                name: "NumberInGroup",
                table: "Achievements");
        }
    }
}
