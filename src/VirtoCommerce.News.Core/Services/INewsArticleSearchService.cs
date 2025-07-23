using VirtoCommerce.News.Core.Models;
using VirtoCommerce.Platform.Core.GenericCrud;

namespace VirtoCommerce.News.Core.Services;

public interface INewsArticleSearchService : ISearchService<NewsArticleSearchCriteria, NewsArticleSearchResult, NewsArticle>
{
}
