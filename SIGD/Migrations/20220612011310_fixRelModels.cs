using Microsoft.EntityFrameworkCore.Migrations;

namespace SIGD.Migrations
{
    public partial class fixRelModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FilesContext_UserUploadId",
                table: "FilesContext");

            migrationBuilder.CreateIndex(
                name: "IX_FilesContext_UserUploadId",
                table: "FilesContext",
                column: "UserUploadId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FilesContext_UserUploadId",
                table: "FilesContext");

            migrationBuilder.CreateIndex(
                name: "IX_FilesContext_UserUploadId",
                table: "FilesContext",
                column: "UserUploadId",
                unique: true);
        }
    }
}
