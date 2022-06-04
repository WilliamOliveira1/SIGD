using Microsoft.EntityFrameworkCore.Migrations;

namespace SIGD.Migrations
{
    public partial class geActivationModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "password",
                table: "ActivationAccount",
                newName: "Password");

            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                table: "ActivationAccount",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext CHARACTER SET utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "ActivationAccount",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext CHARACTER SET utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "adminManager",
                table: "ActivationAccount",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "adminManager",
                table: "ActivationAccount");

            migrationBuilder.RenameColumn(
                name: "Password",
                table: "ActivationAccount",
                newName: "password");

            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                table: "ActivationAccount",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "ActivationAccount",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
