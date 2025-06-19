using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.News.Core.Models;
using VirtoCommerce.News.Core.Services;
using VirtoCommerce.News.ExperienceApi.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Xapi.Core.Infrastructure;

namespace VirtoCommerce.News.ExperienceApi.Queries;

public class NewsArticleContentQueryHandler(INewsArticleService newsArticleService, NewsArticleLocalizedContentService newsArticleLocalizedContentService) : IQueryHandler<NewsArticleContentQuery, NewsArticle>
{
    public async Task<NewsArticle> Handle(NewsArticleContentQuery request, CancellationToken cancellationToken)
    {
        var result = await newsArticleService.GetByIdAsync(request.Id);

        await PostProcessResultAsync(request, result);

        return result;
    }

    protected virtual Task PostProcessResultAsync(NewsArticleContentQuery request, NewsArticle newsArticle)
    {
        newsArticleLocalizedContentService.FilterLanguages(newsArticle, request.LanguageCode);
        return Task.CompletedTask;
    }
}
