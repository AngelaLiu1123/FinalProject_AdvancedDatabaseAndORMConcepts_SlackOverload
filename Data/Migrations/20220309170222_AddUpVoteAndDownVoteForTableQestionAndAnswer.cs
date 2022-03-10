using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinalProject_AdvancedDatabaseAndORMConcepts_SlackOverload.Data.Migrations
{
    public partial class AddUpVoteAndDownVoteForTableQestionAndAnswer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DownVote",
                table: "Questions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UpVote",
                table: "Questions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DownVote",
                table: "Answers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UpVote",
                table: "Answers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DownVote",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "UpVote",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "DownVote",
                table: "Answers");

            migrationBuilder.DropColumn(
                name: "UpVote",
                table: "Answers");
        }
    }
}
