using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.News.Core.Models;
using VirtoCommerce.News.Core.Services;
using VirtoCommerce.News.ExperienceApi.Services;
using VirtoCommerce.Xapi.Core.Infrastructure;

namespace VirtoCommerce.News.ExperienceApi.Queries;

public class NewsArticleContentsQueryHandler(
    INewsArticleSearchService newsArticleSearchService,
    INewsArticleLocalizationService newsArticleLocalizationService,
    INewsArticleSeoService newsArticleSeoService,
    INewsArticleUserGroupsService newsArticleUserGroupsService)
    : IQueryHandler<NewsArticleContentsQuery, NewsArticleSearchResult>
{
    public async Task<NewsArticleSearchResult> Handle(NewsArticleContentsQuery request, CancellationToken cancellationToken)
    {
        var userGroups = await newsArticleUserGroupsService.GetUserGroups(request.UserId);

        var searchCriteria = new NewsArticleSearchCriteria
        {
            LanguageCode = request.LanguageCode,
            Keyword = request.Keyword,
            StoreId = request.StoreId,
            UserGroups = userGroups,
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
        await newsArticleSeoService.FilterSeoInfosAsync(searchResult.Results, request.LanguageCode, request.StoreId);
    }
}
