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
        Func<NewsArticleRepository> repositoryFactory,
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
        var query = ((NewsArticleRepository)repository).NewsArticles;

        if (!criteria.SearchPhrase.IsNullOrEmpty())
        {
            query = query.Where(x => x.Name.Contains(criteria.SearchPhrase));
        }

        if (criteria.Published.HasValue)
        {
            if (criteria.Published.Value)
            {
                query = query.Where(na => na.IsPublished && (!na.PublishDate.HasValue || na.PublishDate >= DateTime.UtcNow));
            }
            else
            {
                query = query.Where(na => !na.IsPublished || (na.PublishDate.HasValue && na.PublishDate < DateTime.UtcNow));
            }
        }

        if (!criteria.StoreId.IsNullOrEmpty())
        {
            query = query.Where(na => na.StoreId == criteria.StoreId);
        }

        if ((criteria.UserGroups != null) && criteria.UserGroups.Any())
        {
            query = query.Where(na => na.UserGroups.Any(ug => criteria.UserGroups.Contains(ug.Group)));
        }

        return query;
    }

    protected override IList<SortInfo> BuildSortExpression(NewsArticleSearchCriteria criteria)
    {
        var sortInfos = criteria.SortInfos;

        if (sortInfos.IsNullOrEmpty())
        {
            if (criteria.Published.GetValueOrDefault())
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
