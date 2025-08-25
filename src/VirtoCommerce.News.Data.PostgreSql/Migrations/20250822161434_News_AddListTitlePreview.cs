using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VirtoCommerce.News.Data.PostgreSql.Migrations
{
    /// <inheritdoc />
    public partial class News_AddListTitlePreview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "NewsArticleLocalizedContent",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "ListPreview",
                table: "NewsArticleLocalizedContent",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ListTitle",
                table: "NewsArticleLocalizedContent",
                type: "character varying(1024)",
                maxLength: 1024,
                nullable: true);

            migrationBuilder.Sql("UPDATE \"NewsArticleLocalizedContent\" SET \"ListPreview\" = \"ContentPreview\",  \"ListTitle\" = \"Title\"");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ListPreview",
                table: "NewsArticleLocalizedContent");

            migrationBuilder.DropColumn(
                name: "ListTitle",
                table: "NewsArticleLocalizedContent");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "NewsArticleLocalizedContent",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
