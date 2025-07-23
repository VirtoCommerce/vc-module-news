using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.News.Data.Models;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.News.Data.Repositories;

public interface INewsArticleRepository : IRepository
{
    IQueryable<NewsArticleLocalizedContentEntity> NewsArticleLocalizedContents { get; }
    IQueryable<NewsArticleEntity> NewsArticles { get; }
    IQueryable<SeoInfoEntity> NewsArticleSeoInfos { get; }
    IQueryable<NewsArticleUserGroupEntity> NewsArticleUserGroups { get; }

    Task<IList<NewsArticleEntity>> GetNewsArticlesByIdsAsync(IList<string> ids);
}
