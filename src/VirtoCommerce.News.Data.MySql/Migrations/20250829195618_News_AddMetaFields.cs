using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VirtoCommerce.News.Data.MySql.Migrations
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
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ListPreview",
                table: "NewsArticleLocalizedContent",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ListTitle",
                table: "NewsArticleLocalizedContent",
                type: "varchar(1024)",
                maxLength: 1024,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "AuthorId",
                table: "NewsArticle",
                type: "varchar(128)",
                maxLength: 128,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "PublishScope",
                table: "NewsArticle",
                type: "varchar(32)",
                maxLength: 32,
                nullable: false,
                defaultValueSql: "'Anonymous'")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "NewsArticleLocalizedTag",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NewsArticleId = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LanguageCode = table.Column<string>(type: "varchar(16)", maxLength: 16, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Tag = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
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
                })
                .Annotation("MySql:CharSet", "utf8mb4");

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

            migrationBuilder.UpdateData(
                table: "NewsArticleLocalizedContent",
                keyColumn: "Content",
                keyValue: null,
                column: "Content",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "NewsArticleLocalizedContent",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
