using Microsoft.EntityFrameworkCore.Migrations;

namespace OpenTimelapseSort.Migrations
{
    public partial class UpdateClassNames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Image_ImageDirectory_ImageDirectoryid",
                table: "Image");

            migrationBuilder.DropForeignKey(
                name: "FK_ImageDirectory_Import_importId",
                table: "ImageDirectory");

            migrationBuilder.DropIndex(
                name: "IX_ImageDirectory_importId",
                table: "ImageDirectory");

            migrationBuilder.RenameColumn(
                name: "ImageDirectoryid",
                table: "Image",
                newName: "SDirectoryid");

            migrationBuilder.RenameIndex(
                name: "IX_Image_ImageDirectoryid",
                table: "Image",
                newName: "IX_Image_SDirectoryid");

            migrationBuilder.AddColumn<int>(
                name: "SImportid",
                table: "ImageDirectory",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ImageDirectory_SImportid",
                table: "ImageDirectory",
                column: "SImportid");

            migrationBuilder.AddForeignKey(
                name: "FK_Image_ImageDirectory_SDirectoryid",
                table: "Image",
                column: "SDirectoryid",
                principalTable: "ImageDirectory",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ImageDirectory_Import_SImportid",
                table: "ImageDirectory",
                column: "SImportid",
                principalTable: "Import",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Image_ImageDirectory_SDirectoryid",
                table: "Image");

            migrationBuilder.DropForeignKey(
                name: "FK_ImageDirectory_Import_SImportid",
                table: "ImageDirectory");

            migrationBuilder.DropIndex(
                name: "IX_ImageDirectory_SImportid",
                table: "ImageDirectory");

            migrationBuilder.DropColumn(
                name: "SImportid",
                table: "ImageDirectory");

            migrationBuilder.RenameColumn(
                name: "SDirectoryid",
                table: "Image",
                newName: "ImageDirectoryid");

            migrationBuilder.RenameIndex(
                name: "IX_Image_SDirectoryid",
                table: "Image",
                newName: "IX_Image_ImageDirectoryid");

            migrationBuilder.CreateIndex(
                name: "IX_ImageDirectory_importId",
                table: "ImageDirectory",
                column: "importId");

            migrationBuilder.AddForeignKey(
                name: "FK_Image_ImageDirectory_ImageDirectoryid",
                table: "Image",
                column: "ImageDirectoryid",
                principalTable: "ImageDirectory",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ImageDirectory_Import_importId",
                table: "ImageDirectory",
                column: "importId",
                principalTable: "Import",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
