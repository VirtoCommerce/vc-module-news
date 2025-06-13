using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.News.Data.Models;
using VirtoCommerce.Platform.Core.Domain;
using VirtoCommerce.Platform.Data.Infrastructure;

namespace VirtoCommerce.News.Data.Repositories;

public class NewsArticleRepository : DbContextRepositoryBase<NewsDbContext>
{
    public NewsArticleRepository(NewsDbContext dbContext, IUnitOfWork unitOfWork = null)
        : base(dbContext, unitOfWork)
    {
    }

    public IQueryable<NewsArticleEntity> NewsArticles => DbContext.Set<NewsArticleEntity>();

    public virtual async Task<IList<NewsArticleEntity>> GetNewsArticlesByIdsAsync(IList<string> ids)
    {
        var result = await NewsArticles
            .Include(na => na.LocalizedContents)
            .Where(na => ids.Contains(na.Id))
            .AsSplitQuery()
            .ToListAsync();

        return result;
    }
}
