using System.Reflection;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.News.Data.Models;
using VirtoCommerce.Platform.Data.Extensions;
using VirtoCommerce.Platform.Data.Infrastructure;

namespace VirtoCommerce.News.Data.Repositories;

public class NewsDbContext : DbContextBase
{
    public NewsDbContext(DbContextOptions<NewsDbContext> options)
        : base(options)
    {
    }

    protected NewsDbContext(DbContextOptions options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<NewsArticleEntity>().ToAuditableEntityTable("NewsArticle");

        modelBuilder.Entity<NewsArticleLocalizedContentEntity>().ToAuditableEntityTable("NewsArticleLocalizedContent");
        modelBuilder.Entity<NewsArticleLocalizedContentEntity>().HasOne(x => x.NewsArticle).WithMany(x => x.LocalizedContents)
             .HasForeignKey(x => x.NewsArticleId).IsRequired().OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<SeoInfoEntity>().ToAuditableEntityTable("NewsArticleSeoInfo");
        modelBuilder.Entity<SeoInfoEntity>().HasOne(x => x.NewsArticle).WithMany(x => x.SeoInfos)
             .HasForeignKey(x => x.NewsArticleId).IsRequired().OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<NewsArticleUserGroupEntity>().ToEntityTable("NewsArticleUserGroup");
        modelBuilder.Entity<NewsArticleUserGroupEntity>().HasOne(x => x.NewsArticle).WithMany(x => x.UserGroups)
            .HasForeignKey(x => x.NewsArticleId).IsRequired().OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<NewsArticleAuthorEntity>().ToEntityTable("NewsArticleAuthor");
        modelBuilder.Entity<NewsArticleEntity>().HasOne(x => x.Author).WithMany(x => x.NewsArticles)
            .HasForeignKey(x => x.AuthorId).OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<NewsArticleLocalizedTagEntity>().ToEntityTable("NewsArticleTag");
        modelBuilder.Entity<NewsArticleLocalizedTagEntity>().HasOne(x => x.NewsArticle).WithMany(x => x.LocalizedTags)
            .HasForeignKey(x => x.NewsArticleId).IsRequired().OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<NewsArticleCommentEntity>().ToEntityTable("NewsArticleComment");
        modelBuilder.Entity<NewsArticleCommentEntity>().HasOne(x => x.NewsArticle).WithMany(x => x.Comments)
            .HasForeignKey(x => x.NewsArticleId).IsRequired().OnDelete(DeleteBehavior.Cascade);

        switch (Database.ProviderName)
        {
            case "Pomelo.EntityFrameworkCore.MySql":
                modelBuilder.ApplyConfigurationsFromAssembly(Assembly.Load("VirtoCommerce.News.Data.MySql"));
                break;
            case "Npgsql.EntityFrameworkCore.PostgreSQL":
                modelBuilder.ApplyConfigurationsFromAssembly(Assembly.Load("VirtoCommerce.News.Data.PostgreSql"));
                break;
            case "Microsoft.EntityFrameworkCore.SqlServer":
                modelBuilder.ApplyConfigurationsFromAssembly(Assembly.Load("VirtoCommerce.News.Data.SqlServer"));
                break;
        }
    }
}
