using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SIGD.Migrations
{
    public partial class changeFileViewModelProperty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UsersToRead",
                table: "FilesContext");

            migrationBuilder.CreateTable(
                name: "FilesViewContext",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    FileModelId = table.Column<Guid>(nullable: false),
                    FileName = table.Column<string>(nullable: true),
                    PrincipalName = table.Column<string>(nullable: true),
                    PrincipalEmail = table.Column<string>(nullable: true),
                    LastTimeOpened = table.Column<DateTime>(nullable: false),
                    Status = table.Column<bool>(nullable: false),
                    Question = table.Column<string>(nullable: true),
                    Answer = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FilesViewContext", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FilesViewContext_FilesContext_FileModelId",
                        column: x => x.FileModelId,
                        principalTable: "FilesContext",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FilesViewContext_FileModelId",
                table: "FilesViewContext",
                column: "FileModelId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FilesViewContext");

            migrationBuilder.AddColumn<string>(
                name: "UsersToRead",
                table: "FilesContext",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);
        }
    }
}
