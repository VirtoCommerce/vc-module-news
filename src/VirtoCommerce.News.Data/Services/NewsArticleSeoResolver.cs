using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.News.Core.Services;
using VirtoCommerce.News.Data.Repositories;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Seo.Core.Models;

namespace VirtoCommerce.News.Data.Services;

public class NewsArticleSeoResolver(Func<INewsArticleRepository> repositoryFactory) : INewsArticleSeoResolver
{
    public async Task<IList<SeoInfo>> FindSeoAsync(SeoSearchCriteria criteria)
    {
        ArgumentNullException.ThrowIfNull(criteria);

        var link = criteria.Slug ?? criteria.Permalink;
        if (link.IsNullOrEmpty())
            return [];

        var lastLinkSegment = link.Split('/', StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
        if (lastLinkSegment.IsNullOrEmpty())
            return [];

        return await FindActiveSeoAsync(lastLinkSegment, criteria.StoreId, criteria.LanguageCode);
    }

    public virtual async Task<IList<SeoInfo>> FindActiveSeoAsync(string lastLinkSegment, string storeId, string languageCode)
    {
        using var repository = repositoryFactory();

        var seoInfoQuery = repository.NewsArticleSeoInfos.Where(x => x.IsActive && x.Keyword == lastLinkSegment);

        if (!storeId.IsNullOrEmpty())
        {
            seoInfoQuery = seoInfoQuery.Where(x => x.StoreId == storeId);
        }

        if (!languageCode.IsNullOrEmpty())
        {
            seoInfoQuery = seoInfoQuery.Where(x => x.LanguageCode == languageCode);
        }

        var seoEntities = await seoInfoQuery.ToListAsync();

        return seoEntities
            .Select(x => x.ToModel(AbstractTypeFactory<SeoInfo>.TryCreateInstance()))
            .ToList();
    }
}
