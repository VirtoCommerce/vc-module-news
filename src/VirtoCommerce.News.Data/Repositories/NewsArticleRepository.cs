using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.News.Data.Models;
using VirtoCommerce.Platform.Core.Domain;
using VirtoCommerce.Platform.Data.Infrastructure;

namespace VirtoCommerce.News.Data.Repositories;

public class NewsArticleRepository(NewsDbContext dbContext, IUnitOfWork unitOfWork = null) : DbContextRepositoryBase<NewsDbContext>(dbContext, unitOfWork), INewsArticleRepository
{
    public IQueryable<NewsArticleEntity> NewsArticles => DbContext.Set<NewsArticleEntity>();

    public IQueryable<NewsArticleLocalizedContentEntity> NewsArticleLocalizedContents => DbContext.Set<NewsArticleLocalizedContentEntity>();

    public IQueryable<SeoInfoEntity> NewsArticleSeoInfos => DbContext.Set<SeoInfoEntity>();

    public IQueryable<NewsArticleUserGroupEntity> NewsArticleUserGroups => DbContext.Set<NewsArticleUserGroupEntity>();

    public IQueryable<NewsArticleLocalizedTagEntity> NewsArticleTags => DbContext.Set<NewsArticleLocalizedTagEntity>();

    public virtual async Task<IList<NewsArticleEntity>> GetNewsArticlesByIdsAsync(IList<string> ids)
    {
        var result = await NewsArticles
            .Where(x => ids.Contains(x.Id))
            .ToListAsync();

        if (result.Count > 0)
        {
            var articleIds = result.Select(x => x.Id).ToList();

            await NewsArticleLocalizedContents
                .Where(x => articleIds.Contains(x.NewsArticleId))
                .LoadAsync();

            await NewsArticleTags
                .Where(x => articleIds.Contains(x.NewsArticleId))
                .LoadAsync();

            await NewsArticleSeoInfos
                .Where(x => articleIds.Contains(x.NewsArticleId))
                .LoadAsync();

            await NewsArticleUserGroups
                .Where(x => articleIds.Contains(x.NewsArticleId))
                .LoadAsync();
        }

        return result;
    }

    public virtual async Task<IList<string>> GetNewsArticlesTagsAsync(string languageCode)
    {
        var query = NewsArticleTags.AsQueryable();

        if (!string.IsNullOrEmpty(languageCode))
        {
            query = query.Where(x => x.LanguageCode == languageCode);
        }

        return await query.Select(x => x.Tag).Distinct().ToListAsync();
    }
}
