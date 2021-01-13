using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace OpenTimelapseSort.Migrations
{
    public partial class SimplifyKeyAttributes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Import",
                columns: table => new
                {
                    id = table.Column<string>(type: "TEXT", nullable: false),
                    target = table.Column<string>(type: "TEXT", nullable: false),
                    name = table.Column<string>(type: "TEXT", nullable: false),
                    timestamp = table.Column<DateTime>(type: "TEXT", nullable: false),
                    importDate = table.Column<string>(type: "TEXT", nullable: false),
                    length = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Import", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ImageDirectory",
                columns: table => new
                {
                    id = table.Column<string>(type: "TEXT", nullable: false),
                    importId = table.Column<string>(type: "TEXT", nullable: true),
                    timestamp = table.Column<DateTime>(type: "TEXT", nullable: false),
                    target = table.Column<string>(type: "TEXT", nullable: false),
                    name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageDirectory", x => x.id);
                    table.ForeignKey(
                        name: "FK_Import_Identifier",
                        column: x => x.importId,
                        principalTable: "Import",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Image",
                columns: table => new
                {
                    id = table.Column<string>(type: "TEXT", nullable: false),
                    directoryId = table.Column<string>(type: "TEXT", nullable: true),
                    name = table.Column<string>(type: "TEXT", nullable: false),
                    target = table.Column<string>(type: "TEXT", nullable: false),
                    fileTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    fileSize = table.Column<long>(type: "INTEGER", nullable: false),
                    parentInstance = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Image", x => x.id);
                    table.ForeignKey(
                        name: "FK_Directory_Identifier",
                        column: x => x.directoryId,
                        principalTable: "ImageDirectory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Image_directoryId",
                table: "Image",
                column: "DirectoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ImageDirectory_importId",
                table: "ImageDirectory",
                column: "ImportId");
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
