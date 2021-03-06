﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace KNI_D6_web.Model.Migrations
{
    public partial class AddToAchievementGroupId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AchievementsGroupId",
                table: "Achievements",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AchievementsGroupId",
                table: "Achievements");
        }
    }
}
