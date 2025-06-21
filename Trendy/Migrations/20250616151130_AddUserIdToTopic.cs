using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trendy.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToTopic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Topics",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Topics");
        }
    }
}
