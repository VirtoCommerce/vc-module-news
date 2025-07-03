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
    protected const string allowedUrlFirstSegment = "news";

    public async Task<IList<SeoInfo>> FindSeoAsync(SeoSearchCriteria criteria)
    {
        var linkSegments = GetLinkSegments(criteria);

        if (!LinkIsValid(linkSegments))
        {
            return [];
        }

        return await FindActiveSeoAsync(linkSegments, criteria.StoreId, criteria.LanguageCode);
    }

    public virtual async Task<IList<SeoInfo>> FindActiveSeoAsync(string[] linkSegments, string storeId, string languageCode)
    {
        var lastLinkSegment = linkSegments.Last();

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

    protected virtual string[] GetLinkSegments(SeoSearchCriteria criteria)
    {
        var link = criteria?.Permalink ?? criteria?.Slug;

        if (link.IsNullOrEmpty())
        {
            return [];
        }

        return link.Split('/', StringSplitOptions.RemoveEmptyEntries);
    }

    protected virtual bool LinkIsValid(string[] linkSegments)
    {
        if ((linkSegments.Length == 0) || (linkSegments.Length > 2) || (linkSegments.Length == 2 && !linkSegments[0].EqualsIgnoreCase(allowedUrlFirstSegment)))
        {
            return false;
        }

        return true;
    }
}
