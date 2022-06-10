using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SIGD.Migrations
{
    public partial class fileModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "FileModelId",
                table: "ActivationAccount",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "FilesContext",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    FileName = table.Column<string>(nullable: true),
                    FileData = table.Column<byte[]>(nullable: true),
                    UserUploadId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FilesContext", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FilesContext_ActivationAccount_UserUploadId",
                        column: x => x.UserUploadId,
                        principalTable: "ActivationAccount",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActivationAccount_FileModelId",
                table: "ActivationAccount",
                column: "FileModelId");

            migrationBuilder.CreateIndex(
                name: "IX_FilesContext_UserUploadId",
                table: "FilesContext",
                column: "UserUploadId");

            migrationBuilder.AddForeignKey(
                name: "FK_ActivationAccount_FilesContext_FileModelId",
                table: "ActivationAccount",
                column: "FileModelId",
                principalTable: "FilesContext",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActivationAccount_FilesContext_FileModelId",
                table: "ActivationAccount");

            migrationBuilder.DropTable(
                name: "FilesContext");

            migrationBuilder.DropIndex(
                name: "IX_ActivationAccount_FileModelId",
                table: "ActivationAccount");

            migrationBuilder.DropColumn(
                name: "FileModelId",
                table: "ActivationAccount");
        }
    }
}
