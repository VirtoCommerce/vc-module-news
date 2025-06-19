using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.News.Core.Models;
using VirtoCommerce.News.Core.Services;
using VirtoCommerce.News.ExperienceApi.Services;
using VirtoCommerce.Xapi.Core.Infrastructure;

namespace VirtoCommerce.News.ExperienceApi.Queries;

public class NewsArticleContentsQueryHandler(INewsArticleSearchService newsArticleSearchService, NewsArticleLocalizedContentService newsArticleLocalizedContentService) : IQueryHandler<NewsArticleContentsQuery, NewsArticleSearchResult>
{
    public async Task<NewsArticleSearchResult> Handle(NewsArticleContentsQuery request, CancellationToken cancellationToken)
    {
        //TODO: user lang then store default lang
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

    protected virtual Task PostProcessResultAsync(NewsArticleContentsQuery request, NewsArticleSearchResult searchResult)
    {
        newsArticleLocalizedContentService.FilterLanguages(searchResult.Results, request.LanguageCode);
        return Task.CompletedTask;
    }
}
