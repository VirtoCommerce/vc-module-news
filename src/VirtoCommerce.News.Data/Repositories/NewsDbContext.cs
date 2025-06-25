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

        modelBuilder.Entity<NewsArticleEntity>().ToAuditableEntityTable("NewsArticle").HasKey(x => x.Id);
        modelBuilder.Entity<NewsArticleEntity>().Property(x => x.Id).HasMaxLength(IdLength).ValueGeneratedOnAdd();

        modelBuilder.Entity<NewsArticleLocalizedContentEntity>().ToAuditableEntityTable("NewsArticleLocalizedContent").HasKey(x => x.Id);
        modelBuilder.Entity<NewsArticleLocalizedContentEntity>().Property(x => x.Id).HasMaxLength(IdLength).ValueGeneratedOnAdd();
        modelBuilder.Entity<NewsArticleLocalizedContentEntity>().HasOne(x => x.NewsArticle).WithMany(x => x.LocalizedContents)
             .HasForeignKey(x => x.NewsArticleId).IsRequired().OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<SeoInfoEntity>().ToAuditableEntityTable("NewsArticleSeoInfo").HasKey(x => x.Id);
        modelBuilder.Entity<SeoInfoEntity>().Property(x => x.Id).HasMaxLength(IdLength).ValueGeneratedOnAdd();
        modelBuilder.Entity<SeoInfoEntity>().HasOne(x => x.NewsArticle).WithMany(x => x.SeoInfos)
             .HasForeignKey(x => x.NewsArticleId).IsRequired().OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<NewsArticleUserGroupEntity>().ToEntityTable("NewsArticleUserGroup").HasKey(x => x.Id);
        modelBuilder.Entity<NewsArticleUserGroupEntity>().Property(x => x.Id).HasMaxLength(IdLength).ValueGeneratedOnAdd();

        modelBuilder.Entity<NewsArticleUserGroupEntity>().HasOne(m => m.NewsArticle).WithMany(m => m.UserGroups)
            .HasForeignKey(m => m.NewsArticleId).IsRequired().OnDelete(DeleteBehavior.Cascade);

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
