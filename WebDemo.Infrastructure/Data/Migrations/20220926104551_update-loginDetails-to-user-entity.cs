using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebDemo.Infrastructure.Data.Migrations
{
    public partial class updateloginDetailstouserentity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Pasword",
                table: "User",
                newName: "Password");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Password",
                table: "User",
                newName: "Pasword");
        }
    }
}
