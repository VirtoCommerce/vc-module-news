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

public class NewsArticleSearchService : SearchService<NewsArticleSearchCriteria, NewsArticleSearchResult, NewsArticle, NewsArticleEntity>, INewsArticleSearchService
{
    public NewsArticleSearchService(
        Func<NewsArticleRepository> repositoryFactory,
        IPlatformMemoryCache platformMemoryCache,
        INewsArticleService crudService,
        IOptions<CrudOptions> crudOptions)
        : base(repositoryFactory, platformMemoryCache, crudService, crudOptions)
    {
    }

    protected override IQueryable<NewsArticleEntity> BuildQuery(IRepository repository, NewsArticleSearchCriteria criteria)
    {
        var query = ((NewsArticleRepository)repository).NewsArticles;

        if (!criteria.SearchPhrase.IsNullOrEmpty())
        {
            query = query.Where(x => x.Name.Contains(criteria.SearchPhrase));
        }

        return query;
    }

    protected override IList<SortInfo> BuildSortExpression(NewsArticleSearchCriteria criteria)
    {
        var sortInfos = criteria.SortInfos;

        if (sortInfos.IsNullOrEmpty())
        {
            sortInfos = [new SortInfo { SortColumn = nameof(NewsArticle.CreatedDate), SortDirection = SortDirection.Descending }];
        }

        return sortInfos;
    }
}
