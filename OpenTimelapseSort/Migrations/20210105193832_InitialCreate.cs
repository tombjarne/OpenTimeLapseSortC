using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OpenTimelapseSort.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Import",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    target = table.Column<string>(type: "TEXT", nullable: true),
                    name = table.Column<string>(type: "TEXT", nullable: true),
                    timestamp = table.Column<DateTime>(type: "TEXT", nullable: false),
                    importDate = table.Column<string>(type: "TEXT", nullable: true),
                    length = table.Column<int>(type: "INTEGER", nullable: false),
                    fetch = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Import", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ImageDirectory",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    importId = table.Column<int>(type: "INTEGER", nullable: false),
                    timestamp = table.Column<DateTime>(type: "TEXT", nullable: false),
                    target = table.Column<string>(type: "TEXT", nullable: false),
                    name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageDirectory", x => x.id);
                    table.ForeignKey(
                        name: "FK_ImageDirectory_Import_importId",
                        column: x => x.importId,
                        principalTable: "Import",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Image",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    directoryId = table.Column<int>(type: "INTEGER", nullable: false),
                    name = table.Column<string>(type: "TEXT", nullable: false),
                    target = table.Column<string>(type: "TEXT", nullable: false),
                    fileTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    fileSize = table.Column<long>(type: "INTEGER", nullable: false),
                    parentInstance = table.Column<string>(type: "TEXT", nullable: false),
                    ImageDirectoryid = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Image", x => x.id);
                    table.ForeignKey(
                        name: "FK_Image_ImageDirectory_ImageDirectoryid",
                        column: x => x.ImageDirectoryid,
                        principalTable: "ImageDirectory",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Image_ImageDirectoryid",
                table: "Image",
                column: "ImageDirectoryid");

            migrationBuilder.CreateIndex(
                name: "IX_ImageDirectory_importId",
                table: "ImageDirectory",
                column: "importId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Image");

            migrationBuilder.DropTable(
                name: "ImageDirectory");

            migrationBuilder.DropTable(
                name: "Import");
        }
    }
}
