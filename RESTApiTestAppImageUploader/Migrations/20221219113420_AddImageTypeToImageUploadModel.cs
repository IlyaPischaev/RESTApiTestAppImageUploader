using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RESTApiTestAppImageUploader.Migrations
{
    /// <inheritdoc />
    public partial class AddImageTypeToImageUploadModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageType",
                table: "ImageUploads",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageType",
                table: "ImageUploads");
        }
    }
}
