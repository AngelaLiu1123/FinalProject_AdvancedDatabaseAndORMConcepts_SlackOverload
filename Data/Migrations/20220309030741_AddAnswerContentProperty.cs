using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinalProject_AdvancedDatabaseAndORMConcepts_SlackOverload.Data.Migrations
{
    public partial class AddAnswerContentProperty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AnswerContent",
                table: "Answers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AnswerContent",
                table: "Answers");
        }
    }
}
