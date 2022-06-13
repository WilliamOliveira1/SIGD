using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SIGD.Migrations
{
    public partial class changeFileModelByteArrayToString : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileData",
                table: "FilesContext");

            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "FilesContext",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "FilesContext");

            migrationBuilder.AddColumn<byte[]>(
                name: "FileData",
                table: "FilesContext",
                type: "longblob",
                nullable: true);
        }
    }
}
