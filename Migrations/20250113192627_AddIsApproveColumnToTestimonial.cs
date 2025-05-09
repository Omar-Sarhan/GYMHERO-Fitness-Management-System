using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gym.Migrations
{
    public partial class AddIsApproveColumnToTestimonial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsApprove",
                schema: "C##OMARSARHAN",
                table: "TESTIMONIAL",
                type: "NUMBER(1)",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsApprove",
                schema: "C##OMARSARHAN",
                table: "TESTIMONIAL");
        }
    }
}
