using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.News.Data.Models;
using VirtoCommerce.Platform.Core.Domain;
using VirtoCommerce.Platform.Data.Infrastructure;

namespace VirtoCommerce.News.Data.Repositories;

public class NewsArticleRepository(NewsDbContext dbContext, IUnitOfWork unitOfWork = null) : DbContextRepositoryBase<NewsDbContext>(dbContext, unitOfWork)
{
    public IQueryable<NewsArticleEntity> NewsArticles => DbContext.Set<NewsArticleEntity>();

    public IQueryable<NewsArticleLocalizedContentEntity> NewsArticleLocalizedContents => DbContext.Set<NewsArticleLocalizedContentEntity>();

    public virtual async Task<IList<NewsArticleEntity>> GetNewsArticlesByIdsAsync(IList<string> ids)
    {
        var result = await NewsArticles
            .Where(na => ids.Contains(na.Id))
            .ToListAsync();

        if (result.Any())
        {
            await NewsArticleLocalizedContents
                .Where(lc => ids.Contains(lc.NewsArticleId))
                .LoadAsync();
        }

        return result;
    }
}
