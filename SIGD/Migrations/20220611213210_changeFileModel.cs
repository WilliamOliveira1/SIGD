using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SIGD.Migrations
{
    public partial class changeFileModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActivationAccount_FilesContext_FileModelId",
                table: "ActivationAccount");

            migrationBuilder.DropForeignKey(
                name: "FK_FilesContext_ActivationAccount_UserUploadId",
                table: "FilesContext");

            migrationBuilder.DropIndex(
                name: "IX_FilesContext_UserUploadId",
                table: "FilesContext");

            migrationBuilder.DropIndex(
                name: "IX_ActivationAccount_FileModelId",
                table: "ActivationAccount");

            migrationBuilder.DropColumn(
                name: "FileModelId",
                table: "ActivationAccount");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserUploadId",
                table: "FilesContext",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UsersToRead",
                table: "FilesContext",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FilesContext_UserUploadId",
                table: "FilesContext",
                column: "UserUploadId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_FilesContext_ActivationAccount_UserUploadId",
                table: "FilesContext",
                column: "UserUploadId",
                principalTable: "ActivationAccount",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FilesContext_ActivationAccount_UserUploadId",
                table: "FilesContext");

            migrationBuilder.DropIndex(
                name: "IX_FilesContext_UserUploadId",
                table: "FilesContext");

            migrationBuilder.DropColumn(
                name: "UsersToRead",
                table: "FilesContext");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserUploadId",
                table: "FilesContext",
                type: "char(36)",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AddColumn<Guid>(
                name: "FileModelId",
                table: "ActivationAccount",
                type: "char(36)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FilesContext_UserUploadId",
                table: "FilesContext",
                column: "UserUploadId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivationAccount_FileModelId",
                table: "ActivationAccount",
                column: "FileModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_ActivationAccount_FilesContext_FileModelId",
                table: "ActivationAccount",
                column: "FileModelId",
                principalTable: "FilesContext",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FilesContext_ActivationAccount_UserUploadId",
                table: "FilesContext",
                column: "UserUploadId",
                principalTable: "ActivationAccount",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
