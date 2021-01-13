using Microsoft.EntityFrameworkCore.Migrations;

namespace OpenTimelapseSort.Migrations
{
    public partial class SimplifyKeyAttributesII : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Import",
                table: "Import");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ImageDirectory",
                table: "ImageDirectory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Image",
                table: "Image");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Import_Identifier",
                table: "Import",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Directory_Identifier",
                table: "ImageDirectory",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Image_Identifier",
                table: "Image",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Import_Identifier",
                table: "Import");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Directory_Identifier",
                table: "ImageDirectory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Image_Identifier",
                table: "Image");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Import",
                table: "Import",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ImageDirectory",
                table: "ImageDirectory",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Image",
                table: "Image",
                column: "Id");
        }
    }
}
