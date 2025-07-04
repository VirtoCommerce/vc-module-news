using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.News.Core.Services;
using VirtoCommerce.News.Data.Repositories;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.Seo.Core.Models;
using NewsSettings = VirtoCommerce.News.Core.ModuleConstants.Settings.General;

namespace VirtoCommerce.News.Data.Services;

public class NewsArticleSeoResolver(Func<INewsArticleRepository> repositoryFactory, ISettingsManager settingsManager) : INewsArticleSeoResolver
{
    public async Task<IList<SeoInfo>> FindSeoAsync(SeoSearchCriteria criteria)
    {
        var linkSegments = GetLinkSegments(criteria);

        if (!await LinkIsValidAsync(linkSegments))
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

    protected virtual async Task<bool> LinkIsValidAsync(string[] linkSegments)
    {
        if (linkSegments.Length != 1)
        {
            return false;
        }

        var useRootLinks = await settingsManager.GetValueAsync<bool>(NewsSettings.UseRootLinks);

        if (!useRootLinks)
        {
            return false;
        }

        return true;
    }
}
