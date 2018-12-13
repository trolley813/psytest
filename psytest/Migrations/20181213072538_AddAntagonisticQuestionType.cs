using Microsoft.EntityFrameworkCore.Migrations;

namespace psytest.Migrations
{
    public partial class AddAntagonisticQuestionType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SliderQuestionType_MaxValue",
                table: "QuestionTypes",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SliderQuestionType_MinValue",
                table: "QuestionTypes",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SliderQuestionType_MaxValue",
                table: "QuestionTypes");

            migrationBuilder.DropColumn(
                name: "SliderQuestionType_MinValue",
                table: "QuestionTypes");
        }
    }
}
