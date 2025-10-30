using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.News.Core.Models;
using VirtoCommerce.Platform.Core.GenericCrud;

namespace VirtoCommerce.News.Core.Services;

public interface INewsArticleIndexedSearchService : ISearchService<NewsArticleIndexedSearchCriteria, NewsArticleSearchResult, NewsArticle>
{
    Task<IList<NewsArticle>> GetByIdsAsync(IList<string> ids);
}
