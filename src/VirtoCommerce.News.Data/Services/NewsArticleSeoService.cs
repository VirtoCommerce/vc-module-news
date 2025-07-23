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

public class NewsArticleSeoService(Func<INewsArticleRepository> repositoryFactory) : INewsArticleSeoService
{
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
            seoInfoQuery = seoInfoQuery.Where(x => x.LanguageCode == null || (x.LanguageCode == languageCode));
        }

        var seoEntities = await seoInfoQuery.ToListAsync();

        return seoEntities
            .OrderBy(x => x.LanguageCode == null)
            .Select(x => x.ToModel(AbstractTypeFactory<SeoInfo>.TryCreateInstance()))
            .ToList();
    }
}
