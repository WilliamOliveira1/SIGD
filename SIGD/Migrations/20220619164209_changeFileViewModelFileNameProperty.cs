using Microsoft.EntityFrameworkCore.Migrations;

namespace SIGD.Migrations
{
    public partial class changeFileViewModelFileNameProperty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileName",
                table: "FilesViewContext");

            migrationBuilder.AddColumn<string>(
                name: "SupervisorName",
                table: "FilesViewContext",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SupervisorName",
                table: "FilesViewContext");

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "FilesViewContext",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);
        }
    }
}
