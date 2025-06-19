using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.News.Core.Models;
using VirtoCommerce.News.Core.Services;
using VirtoCommerce.News.ExperienceApi.Services;
using VirtoCommerce.Xapi.Core.Infrastructure;

namespace VirtoCommerce.News.ExperienceApi.Queries;

public class NewsArticleContentsQueryHandler(INewsArticleSearchService newsArticleSearchService, NewsArticleLocalizationService newsArticleLocalizationService)
    : IQueryHandler<NewsArticleContentsQuery, NewsArticleSearchResult>
{
    public async Task<NewsArticleSearchResult> Handle(NewsArticleContentsQuery request, CancellationToken cancellationToken)
    {
        var searchCriteria = new NewsArticleSearchCriteria
        {
            LanguageCode = request.LanguageCode,
            Keyword = request.Keyword,
            Published = true,
            Sort = nameof(NewsArticle.PublishDate),
            Take = request.Take,
            Skip = request.Skip
        };

        var result = await newsArticleSearchService.SearchAsync(searchCriteria);

        await PostProcessResultAsync(request, result);

        return result;
    }

    protected virtual async Task PostProcessResultAsync(NewsArticleContentsQuery request, NewsArticleSearchResult searchResult)
    {
        await newsArticleLocalizationService.FilterLanguagesAsync(searchResult.Results, request.LanguageCode, request.StoreId);
    }
}
