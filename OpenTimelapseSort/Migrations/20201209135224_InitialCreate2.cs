using Microsoft.EntityFrameworkCore.Migrations;

namespace OpenTimelapseSort.Migrations
{
    public partial class InitialCreate2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Directories_Imports_Importid",
                table: "Directories");

            migrationBuilder.DropForeignKey(
                name: "FK_Image_Directories_ImageDirectoryid",
                table: "Image");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Image",
                table: "Image");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Directories",
                table: "Directories");

            migrationBuilder.RenameTable(
                name: "Image",
                newName: "Images");

            migrationBuilder.RenameTable(
                name: "Directories",
                newName: "ImageDirectories");

            migrationBuilder.RenameIndex(
                name: "IX_Image_ImageDirectoryid",
                table: "Images",
                newName: "IX_Images_ImageDirectoryid");

            migrationBuilder.RenameIndex(
                name: "IX_Directories_Importid",
                table: "ImageDirectories",
                newName: "IX_ImageDirectories_Importid");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Images",
                table: "Images",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ImageDirectories",
                table: "ImageDirectories",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_ImageDirectories_Imports_Importid",
                table: "ImageDirectories",
                column: "Importid",
                principalTable: "Imports",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Images_ImageDirectories_ImageDirectoryid",
                table: "Images",
                column: "ImageDirectoryid",
                principalTable: "ImageDirectories",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImageDirectories_Imports_Importid",
                table: "ImageDirectories");

            migrationBuilder.DropForeignKey(
                name: "FK_Images_ImageDirectories_ImageDirectoryid",
                table: "Images");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Images",
                table: "Images");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ImageDirectories",
                table: "ImageDirectories");

            migrationBuilder.RenameTable(
                name: "Images",
                newName: "Image");

            migrationBuilder.RenameTable(
                name: "ImageDirectories",
                newName: "Directories");

            migrationBuilder.RenameIndex(
                name: "IX_Images_ImageDirectoryid",
                table: "Image",
                newName: "IX_Image_ImageDirectoryid");

            migrationBuilder.RenameIndex(
                name: "IX_ImageDirectories_Importid",
                table: "Directories",
                newName: "IX_Directories_Importid");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Image",
                table: "Image",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Directories",
                table: "Directories",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_Directories_Imports_Importid",
                table: "Directories",
                column: "Importid",
                principalTable: "Imports",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Image_Directories_ImageDirectoryid",
                table: "Image",
                column: "ImageDirectoryid",
                principalTable: "Directories",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
