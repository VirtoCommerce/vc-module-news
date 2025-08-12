using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VirtoCommerce.News.Data.PostgreSql.Migrations
{
    /// <inheritdoc />
    public partial class News_AuthorTagsComments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AuthorId",
                table: "NewsArticle",
                type: "character varying(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsSharingAllowed",
                table: "NewsArticle",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "NewsArticleAuthor",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    PhotoUrl = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    ModifiedBy = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsArticleAuthor", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NewsArticleComment",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    NewsArticleId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Text = table.Column<string>(type: "text", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    ModifiedBy = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsArticleComment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NewsArticleComment_NewsArticle_NewsArticleId",
                        column: x => x.NewsArticleId,
                        principalTable: "NewsArticle",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NewsArticleTag",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    NewsArticleId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Tag = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsArticleTag", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NewsArticleTag_NewsArticle_NewsArticleId",
                        column: x => x.NewsArticleId,
                        principalTable: "NewsArticle",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NewsArticle_AuthorId",
                table: "NewsArticle",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_NewsArticleComment_NewsArticleId",
                table: "NewsArticleComment",
                column: "NewsArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_NewsArticleTag_NewsArticleId",
                table: "NewsArticleTag",
                column: "NewsArticleId");

            migrationBuilder.AddForeignKey(
                name: "FK_NewsArticle_NewsArticleAuthor_AuthorId",
                table: "NewsArticle",
                column: "AuthorId",
                principalTable: "NewsArticleAuthor",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NewsArticle_NewsArticleAuthor_AuthorId",
                table: "NewsArticle");

            migrationBuilder.DropTable(
                name: "NewsArticleAuthor");

            migrationBuilder.DropTable(
                name: "NewsArticleComment");

            migrationBuilder.DropTable(
                name: "NewsArticleTag");

            migrationBuilder.DropIndex(
                name: "IX_NewsArticle_AuthorId",
                table: "NewsArticle");

            migrationBuilder.DropColumn(
                name: "AuthorId",
                table: "NewsArticle");

            migrationBuilder.DropColumn(
                name: "IsSharingAllowed",
                table: "NewsArticle");
        }
    }
}
