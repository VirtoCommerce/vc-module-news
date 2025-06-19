using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.News.Core.Models;
using VirtoCommerce.News.Core.Services;
using VirtoCommerce.News.ExperienceApi.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Xapi.Core.Infrastructure;

namespace VirtoCommerce.News.ExperienceApi.Queries;

public class NewsArticleContentQueryHandler(INewsArticleService newsArticleService, NewsArticleLocalizationService newsArticleLocalizationService)
    : IQueryHandler<NewsArticleContentQuery, NewsArticle>
{
    public async Task<NewsArticle> Handle(NewsArticleContentQuery request, CancellationToken cancellationToken)
    {
        var result = await newsArticleService.GetByIdAsync(request.Id);

        await PostProcessResultAsync(request, result);

        return result;
    }

    protected virtual async Task PostProcessResultAsync(NewsArticleContentQuery request, NewsArticle newsArticle)
    {
        await newsArticleLocalizationService.FilterLanguagesAsync([newsArticle], request.LanguageCode, request.StoreId);
    }
}
