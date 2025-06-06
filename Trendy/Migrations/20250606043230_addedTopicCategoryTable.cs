using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trendy.Migrations
{
    /// <inheritdoc />
    public partial class addedTopicCategoryTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.CategoryId);
                });

            migrationBuilder.CreateTable(
                name: "CategoryTopic",
                columns: table => new
                {
                    CategoriesCategoryId = table.Column<int>(type: "int", nullable: false),
                    TopicsTopicId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryTopic", x => new { x.CategoriesCategoryId, x.TopicsTopicId });
                    table.ForeignKey(
                        name: "FK_CategoryTopic_Category_CategoriesCategoryId",
                        column: x => x.CategoriesCategoryId,
                        principalTable: "Category",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CategoryTopic_Topics_TopicsTopicId",
                        column: x => x.TopicsTopicId,
                        principalTable: "Topics",
                        principalColumn: "TopicId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CategoryTopic_TopicsTopicId",
                table: "CategoryTopic",
                column: "TopicsTopicId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CategoryTopic");

            migrationBuilder.DropTable(
                name: "Category");
        }
    }
}
