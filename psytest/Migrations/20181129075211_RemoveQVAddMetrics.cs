using Microsoft.EntityFrameworkCore.Migrations;

namespace psytest.Migrations
{
    public partial class RemoveQVAddMetrics : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QuestionVariant");

            migrationBuilder.AddColumn<string>(
                name: "MetricsComputeScript",
                table: "Tests",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetricsDescriptions",
                table: "Tests",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Variants",
                table: "QuestionTypes",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MetricsComputeScript",
                table: "Tests");

            migrationBuilder.DropColumn(
                name: "MetricsDescriptions",
                table: "Tests");

            migrationBuilder.DropColumn(
                name: "Variants",
                table: "QuestionTypes");

            migrationBuilder.CreateTable(
                name: "QuestionVariant",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Text = table.Column<string>(nullable: true),
                    VariantQuestionTypeId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionVariant", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionVariant_QuestionTypes_VariantQuestionTypeId",
                        column: x => x.VariantQuestionTypeId,
                        principalTable: "QuestionTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QuestionVariant_VariantQuestionTypeId",
                table: "QuestionVariant",
                column: "VariantQuestionTypeId");
        }
    }
}
