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

        if (criteria.Published.HasValue)
        {
            var utcNow = criteria.CertainDate.GetValueOrDefault(DateTime.UtcNow);

            if (criteria.Published.Value)
            {
                query = query.Where(x => x.IsPublished && (x.PublishDate == null || x.PublishDate <= utcNow));
            }
            else
            {
                query = query.Where(x => !x.IsPublished || (x.PublishDate != null && x.PublishDate > utcNow));
            }
        }

        if (!criteria.StoreId.IsNullOrEmpty())
        {
            query = query.Where(x => x.StoreId == criteria.StoreId);
        }

        if (criteria.UserGroups != null)
        {
            query = query.Where(article => !article.UserGroups.Any() || article.UserGroups.Any(group => criteria.UserGroups.Contains(group.Group)));
        }

        if (criteria.LanguageCodes != null)
        {
            query = query.Where(article => article.LocalizedContents.Any(content => criteria.LanguageCodes.Contains(content.LanguageCode)));
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
