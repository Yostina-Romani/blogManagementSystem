using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class posttt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_posts_users_UsersuserId",
                table: "posts");

            migrationBuilder.DropIndex(
                name: "IX_posts_UsersuserId",
                table: "posts");

            migrationBuilder.DropColumn(
                name: "UsersuserId",
                table: "posts");

            migrationBuilder.CreateIndex(
                name: "IX_posts_userId",
                table: "posts",
                column: "userId");

            migrationBuilder.AddForeignKey(
                name: "FK_posts_users_userId",
                table: "posts",
                column: "userId",
                principalTable: "users",
                principalColumn: "userId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_posts_users_userId",
                table: "posts");

            migrationBuilder.DropIndex(
                name: "IX_posts_userId",
                table: "posts");

            migrationBuilder.AddColumn<int>(
                name: "UsersuserId",
                table: "posts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_posts_UsersuserId",
                table: "posts",
                column: "UsersuserId");

            migrationBuilder.AddForeignKey(
                name: "FK_posts_users_UsersuserId",
                table: "posts",
                column: "UsersuserId",
                principalTable: "users",
                principalColumn: "userId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
