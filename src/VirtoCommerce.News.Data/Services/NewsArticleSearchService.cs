using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using VirtoCommerce.News.Core.Models;
using VirtoCommerce.News.Core.Services;
using VirtoCommerce.News.Data.Models;
using VirtoCommerce.News.Data.Repositories;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.GenericCrud;
using VirtoCommerce.Platform.Data.GenericCrud;

namespace VirtoCommerce.News.Data.Services;

public class NewsArticleSearchService(
        Func<INewsArticleRepository> repositoryFactory,
        IPlatformMemoryCache platformMemoryCache,
        INewsArticleService crudService,
        IOptions<CrudOptions> crudOptions)
    : SearchService<NewsArticleSearchCriteria, NewsArticleSearchResult, NewsArticle, NewsArticleEntity>(
        repositoryFactory,
        platformMemoryCache,
        crudService,
        crudOptions),
    INewsArticleSearchService
{
    protected override IQueryable<NewsArticleEntity> BuildQuery(IRepository repository, NewsArticleSearchCriteria criteria)
    {
        var query = ((INewsArticleRepository)repository).NewsArticles;

        if (!criteria.SearchPhrase.IsNullOrEmpty())
        {
            query = query.Where(x => x.Name.Contains(criteria.SearchPhrase));
        }

        if (!criteria.StoreId.IsNullOrEmpty())
        {
            query = query.Where(x => x.StoreId == criteria.StoreId);
        }

        if (criteria.LanguageCodes != null)
        {
            query = query.Where(article => article.LocalizedContents.Any(content => criteria.LanguageCodes.Contains(content.LanguageCode)));
        }

        if (!criteria.AuthorId.IsNullOrEmpty())
        {
            query = query.Where(x => x.AuthorId == criteria.AuthorId);
        }

        if (criteria.Tags != null)
        {
            query = query.Where(article => article.LocalizedTags.Any(tag => criteria.Tags.Contains(tag.Tag)));
        }

        if (!criteria.ContentKeyword.IsNullOrEmpty())
        {
            query = BuildContentKeywordSearchCriteria(query, criteria);
        }

        if (criteria.Status.HasValue)
        {
            query = BuildStatusSearchCriteria(query, criteria);
        }

        if (!criteria.PublishScope.IsNullOrEmpty())
        {
            query = BuildPublishScopeQuery(query, criteria);
        }

        return query;
    }

    protected virtual IQueryable<NewsArticleEntity> BuildContentKeywordSearchCriteria(IQueryable<NewsArticleEntity> query, NewsArticleSearchCriteria criteria)
    {
        query = query
            .Where(article => article.LocalizedContents
                .Any(content => content.Title.Contains(criteria.ContentKeyword)
                    || content.Content.Contains(criteria.ContentKeyword)
                    || content.ContentPreview.Contains(criteria.ContentKeyword)
                    || content.ListTitle.Contains(criteria.ContentKeyword)
                    || content.ListPreview.Contains(criteria.ContentKeyword)));

        return query;
    }

    protected virtual IQueryable<NewsArticleEntity> BuildStatusSearchCriteria(IQueryable<NewsArticleEntity> query, NewsArticleSearchCriteria criteria)
    {
        var utcNow = criteria.CertainDate.GetValueOrDefault(DateTime.UtcNow);

        if (criteria.Status == NewsArticleStatus.Published)
        {
            query = query.Where(x => x.IsPublished && (x.PublishDate == null || x.PublishDate <= utcNow));
        }
        else if (criteria.Status == NewsArticleStatus.Scheduled)
        {
            query = query.Where(x => x.IsPublished && (x.PublishDate != null && x.PublishDate > utcNow));
        }
        else if (criteria.Status == NewsArticleStatus.Archived)
        {
            query = query.Where(x => x.IsArchived && (x.ArchiveDate == null || x.ArchiveDate <= utcNow));
        }
        else if (criteria.Status == NewsArticleStatus.Draft)
        {
            query = query.Where(x => !x.IsPublished && !x.IsArchived);
        }

        return query;
    }

    protected virtual IQueryable<NewsArticleEntity> BuildPublishScopeQuery(IQueryable<NewsArticleEntity> query, NewsArticleSearchCriteria criteria)
    {
        if (criteria.PublishScope == NewsArticlePublishScope.Anonymous)
        {
            query = query.Where(x => x.PublishScope == NewsArticlePublishScope.Anonymous);
        }
        else if (criteria.PublishScope == NewsArticlePublishScope.Authorized)
        {
            if (criteria.UserGroups != null)
            {
                query = query.Where(article =>
                    (article.PublishScope == NewsArticlePublishScope.Anonymous) ||
                    (article.PublishScope == NewsArticlePublishScope.Authorized && (!article.UserGroups.Any() || article.UserGroups.Any(group => criteria.UserGroups.Contains(group.Group)))));
            }
            else
            {
                query = query.Where(x => x.PublishScope == NewsArticlePublishScope.Authorized || x.PublishScope == NewsArticlePublishScope.Anonymous);
            }
        }

        return query;
    }

    protected override IList<SortInfo> BuildSortExpression(NewsArticleSearchCriteria criteria)
    {
        var sortInfos = criteria.SortInfos;

        if (sortInfos.IsNullOrEmpty())
        {
            if (criteria.Status == NewsArticleStatus.Published)
            {
                sortInfos = [new SortInfo { SortColumn = nameof(NewsArticle.PublishDate), SortDirection = SortDirection.Descending }];
            }
            else
            {
                sortInfos = [new SortInfo { SortColumn = nameof(NewsArticle.CreatedDate), SortDirection = SortDirection.Descending }];
            }
        }

        return sortInfos;
    }
}
