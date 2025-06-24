using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentValidation;
using VirtoCommerce.News.Core.Events;
using VirtoCommerce.News.Core.Models;
using VirtoCommerce.News.Core.Services;
using VirtoCommerce.News.Data.Models;
using VirtoCommerce.News.Data.Repositories;
using VirtoCommerce.News.Data.Validation;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Data.GenericCrud;

namespace VirtoCommerce.News.Data.Services;

public class NewsArticleService(
        Func<INewsArticleRepository> repositoryFactory,
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
        return ((INewsArticleRepository)repository).GetNewsArticlesByIdsAsync(ids);
    }

    protected override async Task BeforeSaveChanges(IList<NewsArticle> models)
    {
        await base.BeforeSaveChanges(models);
        await Validate(models);
    }

    protected virtual async Task Validate(IList<NewsArticle> newsArticles)
    {
        var validator = new NewsArticleValidator();

        foreach (var newsArticle in newsArticles)
        {
            await validator.ValidateAndThrowAsync(newsArticle);
        }
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
