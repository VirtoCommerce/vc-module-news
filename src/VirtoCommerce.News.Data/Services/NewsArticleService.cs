using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentValidation;
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
        Func<INewsArticleRepository> repositoryFactory,
        IPlatformMemoryCache platformMemoryCache,
        IEventPublisher eventPublisher,
        AbstractValidator<NewsArticle> newsArticleValidator)
    : CrudService<NewsArticle, NewsArticleEntity, NewsArticleChangingEvent, NewsArticleChangedEvent>(
        repositoryFactory,
        platformMemoryCache,
        eventPublisher),
    INewsArticleService
{
    protected const string ClonedNewsArticlePrefix = "[COPY] ";

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
        foreach (var newsArticle in newsArticles)
        {
            await newsArticleValidator.ValidateAndThrowAsync(newsArticle);
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

    protected virtual async Task ChangeIsPublishedAsync(IList<string> ids, bool isPublished)
    {
        var newsArticles = await GetAsync(ids);

        foreach (var newsArticle in newsArticles)
        {
            newsArticle.SetIsPublished(isPublished);
            if (isPublished && !newsArticle.PublishDate.HasValue)
            {
                newsArticle.PublishDate = DateTime.UtcNow;
            }
        }

        await SaveChangesAsync(newsArticles);
    }

    public async Task ArchiveAsync(IList<string> ids)
    {
        await ChangeIsArchivedAsync(ids, true);
    }

    public async Task UnarchiveAsync(IList<string> ids)
    {
        await ChangeIsArchivedAsync(ids, false);
    }

    protected virtual async Task ChangeIsArchivedAsync(IList<string> ids, bool isArchived)
    {
        var newsArticles = await GetAsync(ids);

        foreach (var newsArticle in newsArticles)
        {
            newsArticle.SetIsArchived(isArchived);
        }

        await SaveChangesAsync(newsArticles);
    }

    public virtual async Task<NewsArticle> Clone(NewsArticle newsArticle)
    {
        var clonedNewsArticle = newsArticle.CloneTyped();

        MakeCloneable(clonedNewsArticle);
        SetClonedDefaultValues(clonedNewsArticle);

        await SaveChangesAsync([clonedNewsArticle]);

        return clonedNewsArticle;
    }

    protected void MakeCloneable(NewsArticle newsArticle)
    {
        newsArticle.Id = null;

        foreach (var localizedContent in newsArticle.LocalizedContents)
        {
            localizedContent.NewsArticleId = null;
            localizedContent.Id = null;
        }

        foreach (var seoInfo in newsArticle.SeoInfos)
        {
            seoInfo.Id = null;
        }
    }

    protected virtual void SetClonedDefaultValues(NewsArticle newsArticle)
    {
        newsArticle.IsPublished = false;
        newsArticle.PublishDate = null;
        newsArticle.Name = ClonedNewsArticlePrefix + newsArticle.Name;

        foreach (var seoInfo in newsArticle.SeoInfos)
        {
            seoInfo.IsActive = false;
        }
    }
}
