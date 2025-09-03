using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VirtoCommerce.News.Data.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class News_AddMetaFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "NewsArticleLocalizedContent",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "ListPreview",
                table: "NewsArticleLocalizedContent",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ListTitle",
                table: "NewsArticleLocalizedContent",
                type: "nvarchar(1024)",
                maxLength: 1024,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AuthorId",
                table: "NewsArticle",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PublishScope",
                table: "NewsArticle",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: false,
                defaultValueSql: "'Anonymous'");

            migrationBuilder.CreateTable(
                name: "NewsArticleLocalizedTag",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    NewsArticleId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    LanguageCode = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    Tag = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsArticleLocalizedTag", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NewsArticleLocalizedTag_NewsArticle_NewsArticleId",
                        column: x => x.NewsArticleId,
                        principalTable: "NewsArticle",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NewsArticleLocalizedTag_NewsArticleId",
                table: "NewsArticleLocalizedTag",
                column: "NewsArticleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NewsArticleLocalizedTag");

            migrationBuilder.DropColumn(
                name: "ListPreview",
                table: "NewsArticleLocalizedContent");

            migrationBuilder.DropColumn(
                name: "ListTitle",
                table: "NewsArticleLocalizedContent");

            migrationBuilder.DropColumn(
                name: "AuthorId",
                table: "NewsArticle");

            migrationBuilder.DropColumn(
                name: "PublishScope",
                table: "NewsArticle");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "NewsArticleLocalizedContent",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
