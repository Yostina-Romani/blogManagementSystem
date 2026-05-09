using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class jkdfjk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserPassword_confirm",
                table: "users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserPassword_confirm",
                table: "users");
        }
    }
}
