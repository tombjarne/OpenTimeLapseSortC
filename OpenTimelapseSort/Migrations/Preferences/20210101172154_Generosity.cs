using Microsoft.EntityFrameworkCore.Migrations;

namespace OpenTimelapseSort.Migrations.Preferences
{
    public partial class Generosity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Preferences",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    useCopy = table.Column<bool>(type: "INTEGER", nullable: false),
                    useAutoDetectInterval = table.Column<bool>(type: "INTEGER", nullable: false),
                    sequenceInterval = table.Column<double>(type: "REAL", nullable: false),
                    sequenceIntervalGenerosity = table.Column<int>(type: "INTEGER", nullable: false),
                    sequenceImageCount = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Preferences", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Preferences");
        }
    }
}
