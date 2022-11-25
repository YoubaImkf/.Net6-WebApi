using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebDemo.Infrastructure.Data.Migrations
{
    public partial class addloginDetailstouserentity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Pasword",
                table: "User",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Username",
                table: "User",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Pasword",
                table: "User");

            migrationBuilder.DropColumn(
                name: "Username",
                table: "User");
        }
    }
}
