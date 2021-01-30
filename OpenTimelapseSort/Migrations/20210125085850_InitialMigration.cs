using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace OpenTimelapseSort.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                "Import",
                table => new
                {
                    Id = table.Column<string>("TEXT", nullable: false),
                    Origin = table.Column<string>("TEXT", nullable: false),
                    Name = table.Column<string>("TEXT", nullable: false),
                    Timestamp = table.Column<DateTime>("TEXT", nullable: false),
                    ImportDate = table.Column<string>("TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Import", x => x.Id);
                });

            migrationBuilder.CreateTable(
                "ImageDirectory",
                table => new
                {
                    Id = table.Column<string>("TEXT", nullable: false),
                    ImportId = table.Column<string>("TEXT", nullable: true),
                    Timestamp = table.Column<DateTime>("TEXT", nullable: false),
                    Origin = table.Column<string>("TEXT", nullable: false),
                    Target = table.Column<string>("TEXT", nullable: true),
                    Name = table.Column<string>("TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageDirectory", x => x.Id);
                    table.ForeignKey(
                        "FK_Import_Identifier",
                        x => x.ImportId,
                        "Import",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "Image",
                table => new
                {
                    Id = table.Column<string>("TEXT", nullable: false),
                    DirectoryId = table.Column<string>("TEXT", nullable: true),
                    Name = table.Column<string>("TEXT", nullable: false),
                    Origin = table.Column<string>("TEXT", nullable: false),
                    Target = table.Column<string>("TEXT", nullable: true),
                    FileTime = table.Column<long>("INTEGER", nullable: false),
                    FileSize = table.Column<long>("INTEGER", nullable: false),
                    ParentInstance = table.Column<string>("TEXT", nullable: true),
                    Lumen = table.Column<double>("REAL", nullable: false),
                    Colors = table.Column<long>("INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Image", x => x.Id);
                    table.ForeignKey(
                        "FK_Directory_Identifier",
                        x => x.DirectoryId,
                        "ImageDirectory",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                "IX_Image_DirectoryId",
                "Image",
                "DirectoryId");

            migrationBuilder.CreateIndex(
                "IX_ImageDirectory_ImportId",
                "ImageDirectory",
                "ImportId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "Image");

            migrationBuilder.DropTable(
                "ImageDirectory");

            migrationBuilder.DropTable(
                "Import");
        }
    }
}
