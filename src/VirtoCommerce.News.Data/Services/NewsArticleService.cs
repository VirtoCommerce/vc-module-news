using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.News.Core.Events;
using VirtoCommerce.News.Core.Models;
using VirtoCommerce.News.Core.Services;
using VirtoCommerce.News.Data.Models;
using VirtoCommerce.News.Data.Repositories;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Data.GenericCrud;

namespace VirtoCommerce.News.Data.Services;

public class NewsArticleService(
        Func<NewsArticleRepository> repositoryFactory,
        IPlatformMemoryCache platformMemoryCache,
        IEventPublisher eventPublisher)
    : CrudService<NewsArticle, NewsArticleEntity, NewsArticleChangingEvent, NewsArticleChangedEvent>(
        repositoryFactory,
        platformMemoryCache,
        eventPublisher),
    INewsArticleService
{
    protected override Task<IList<NewsArticleEntity>> LoadEntities(IRepository repository, IList<string> ids, string responseGroup)
    {
        return ((NewsArticleRepository)repository).GetNewsArticlesByIdsAsync(ids);
    }

    public async Task PublishAsync(IList<string> ids)
    {
        await ChangeIsPublishedAsync(ids, true);
    }

    public async Task UnpublishAsync(IList<string> ids)
    {
        await ChangeIsPublishedAsync(ids, false);
    }

    private async Task ChangeIsPublishedAsync(IList<string> ids, bool isPublished)
    {
        var newsArticles = await GetAsync(ids);
        foreach (var newsArticle in newsArticles)
        {
            newsArticle.SetIsPublished(isPublished);
        }
        await SaveChangesAsync(newsArticles);
    }
}
