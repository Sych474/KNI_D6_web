using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace KNI_D6_web.Model.Migrations
{
    public partial class AddSemester : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SemesterId",
                table: "Events",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SemesterId",
                table: "Achievements",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Semesters",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Semesters", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Events_SemesterId",
                table: "Events",
                column: "SemesterId");

            migrationBuilder.CreateIndex(
                name: "IX_Achievements_SemesterId",
                table: "Achievements",
                column: "SemesterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Achievements_Semesters_SemesterId",
                table: "Achievements",
                column: "SemesterId",
                principalTable: "Semesters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Semesters_SemesterId",
                table: "Events",
                column: "SemesterId",
                principalTable: "Semesters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Achievements_Semesters_SemesterId",
                table: "Achievements");

            migrationBuilder.DropForeignKey(
                name: "FK_Events_Semesters_SemesterId",
                table: "Events");

            migrationBuilder.DropTable(
                name: "Semesters");

            migrationBuilder.DropIndex(
                name: "IX_Events_SemesterId",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Achievements_SemesterId",
                table: "Achievements");

            migrationBuilder.DropColumn(
                name: "SemesterId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "SemesterId",
                table: "Achievements");
        }
    }
}
