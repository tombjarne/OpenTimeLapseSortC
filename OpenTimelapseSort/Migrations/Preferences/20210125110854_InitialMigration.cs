using Microsoft.EntityFrameworkCore.Migrations;

namespace OpenTimelapseSort.Migrations.Preferences
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                "Preferences",
                table => new
                {
                    Id = table.Column<int>("INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UseCopy = table.Column<bool>("INTEGER", nullable: false),
                    UseAutoDetectInterval = table.Column<bool>("INTEGER", nullable: false),
                    SequenceInterval = table.Column<double>("REAL", nullable: false),
                    SequenceIntervalGenerosity = table.Column<int>("INTEGER", nullable: false),
                    SequenceImageCount = table.Column<int>("INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Preferences", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "Preferences");
        }
    }
}
