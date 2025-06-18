using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.News.Core.Models;
using VirtoCommerce.News.Core.Services;
using VirtoCommerce.Xapi.Core.Infrastructure;

namespace VirtoCommerce.News.ExperienceApi.Queries;

public class NewsArticlesQueryHandler(INewsArticleSearchService newsArticleSearchService) : IQueryHandler<NewsArticlesQuery, NewsArticleSearchResult>
{
    public async Task<NewsArticleSearchResult> Handle(NewsArticlesQuery request, CancellationToken cancellationToken)
    {
        var result = await newsArticleSearchService.SearchAsync(new NewsArticleSearchCriteria() { Keyword = request.Keyword });
        return result;
    }
}
