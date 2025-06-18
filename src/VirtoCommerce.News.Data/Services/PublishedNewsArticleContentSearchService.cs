using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

public class PublishedNewsArticleContentSearchService : SearchService<NewsArticleContentSearchCriteria, NewsArticleSearchResult, NewsArticle, NewsArticleEntity>, IPublishedNewsArticleContentSearchService
{
    public PublishedNewsArticleContentSearchService(
        Func<NewsArticleRepository> repositoryFactory,
        IPlatformMemoryCache platformMemoryCache,
        INewsArticleService crudService,
        IOptions<CrudOptions> crudOptions)
        : base(repositoryFactory, platformMemoryCache, crudService, crudOptions)
    {
    }

    protected override IQueryable<NewsArticleEntity> BuildQuery(IRepository repository, NewsArticleContentSearchCriteria criteria)
    {
        var query = ((NewsArticleRepository)repository).NewsArticles;

        if (!criteria.Id.IsNullOrEmpty())
        {
            query = query.Where(x => x.Id == criteria.Id);
            return query;
        }

        query = query.Where(na => na.IsPublished && (!na.PublishDate.HasValue || na.PublishDate >= DateTime.UtcNow));

        query = query
            .Where(na => na.LocalizedContents
                .Any(lc => lc.LanguageCode == criteria.LanguageCode));

        if (!criteria.SearchPhrase.IsNullOrEmpty())
        {
            query = query
                .Where(na => na.LocalizedContents
                    .Where(lc => lc.LanguageCode == criteria.LanguageCode)
                    .Any(lc => lc.Title.Contains(criteria.SearchPhrase)));
        }

        return query;
    }

    protected override async Task<NewsArticleSearchResult> ProcessSearchResultAsync(NewsArticleSearchResult result, NewsArticleContentSearchCriteria criteria)
    {
        result = await base.ProcessSearchResultAsync(result, criteria);

        foreach (var item in result.Results)
        {
            item.LocalizedContents = item.LocalizedContents
                .Where(x => String.Equals(x.LanguageCode, criteria.LanguageCode, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        return result;
    }

    protected override IList<SortInfo> BuildSortExpression(NewsArticleContentSearchCriteria criteria)
    {
        var sortInfos = criteria.SortInfos;

        if (sortInfos.IsNullOrEmpty())
        {
            sortInfos = [new SortInfo { SortColumn = nameof(NewsArticle.PublishDate), SortDirection = SortDirection.Descending }];
        }

        return sortInfos;
    }
}
