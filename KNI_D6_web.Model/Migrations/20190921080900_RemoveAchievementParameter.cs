using Microsoft.EntityFrameworkCore.Migrations;

namespace KNI_D6_web.Model.Migrations
{
    public partial class RemoveAchievementParameter : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AchievementParameters");

            migrationBuilder.AddColumn<int>(
                name: "ParameterId",
                table: "Achievements",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Achievements_ParameterId",
                table: "Achievements",
                column: "ParameterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Achievements_Parameters_ParameterId",
                table: "Achievements",
                column: "ParameterId",
                principalTable: "Parameters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Achievements_Parameters_ParameterId",
                table: "Achievements");

            migrationBuilder.DropIndex(
                name: "IX_Achievements_ParameterId",
                table: "Achievements");

            migrationBuilder.DropColumn(
                name: "ParameterId",
                table: "Achievements");

            migrationBuilder.CreateTable(
                name: "AchievementParameters",
                columns: table => new
                {
                    ParameterId = table.Column<int>(nullable: false),
                    AchievementId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AchievementParameters", x => new { x.ParameterId, x.AchievementId });
                    table.ForeignKey(
                        name: "FK_AchievementParameters_Achievements_AchievementId",
                        column: x => x.AchievementId,
                        principalTable: "Achievements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AchievementParameters_Parameters_ParameterId",
                        column: x => x.ParameterId,
                        principalTable: "Parameters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AchievementParameters_AchievementId",
                table: "AchievementParameters",
                column: "AchievementId");
        }
    }
}
