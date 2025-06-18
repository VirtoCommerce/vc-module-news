using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.News.Core.Models;
using VirtoCommerce.News.Core.Services;
using VirtoCommerce.News.ExperienceApi.Models;
using VirtoCommerce.Xapi.Core.Infrastructure;

namespace VirtoCommerce.News.ExperienceApi.Queries;

public class NewsArticlesContentQueryHandler(IPublishedNewsArticleContentSearchService publishedNewsArticleContentSearchService) : IQueryHandler<NewsArticlesContentQuery, NewsArticleContentSearchResult>
{
    public async Task<NewsArticleContentSearchResult> Handle(NewsArticlesContentQuery request, CancellationToken cancellationToken)
    {
        var searchCriteria = new NewsArticleContentSearchCriteria
        {
            Id = request.Id,
            LanguageCode = request.LanguageCode,
            Keyword = request.Keyword
        };

        var searchResult = await publishedNewsArticleContentSearchService.SearchAsync(searchCriteria);

        var result = new NewsArticleContentSearchResult()
        {
            TotalCount = searchResult.TotalCount,
            Results = searchResult.Results
                .Select(x => new NewsArticleContent
                {
                    Id = x.Id,
                    PublishedDate = x.PublishDate,
                    Title = x.LocalizedContents.FirstOrDefault()?.Title,
                    LocalizedContent = x.LocalizedContents.FirstOrDefault()?.Content,
                    LocalizedContentPreview = x.LocalizedContents.FirstOrDefault()?.ContentPreview
                })
                .ToList()
        };

        return result;
    }
}
