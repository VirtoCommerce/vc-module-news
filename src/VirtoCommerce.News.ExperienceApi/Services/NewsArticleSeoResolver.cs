using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.News.Core.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Seo.Core.Models;
using VirtoCommerce.Seo.Core.Services;

namespace VirtoCommerce.News.ExperienceApi.Services;

public class NewsArticleSeoResolver(INewsArticleSeoService newsArticleSeoService, INewsArticleSettingsService newsArticleSettingsService) : ISeoResolver
{
    public async Task<IList<SeoInfo>> FindSeoAsync(SeoSearchCriteria criteria)
    {
        var linkSegments = GetLinkSegments(criteria);

        if (!await LinkIsValidAsync(linkSegments, criteria.StoreId))
        {
            return [];
        }

        return await newsArticleSeoService.FindActiveSeoAsync(linkSegments, criteria.StoreId, criteria.LanguageCode);
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

    protected virtual async Task<bool> LinkIsValidAsync(string[] linkSegments, string storeId)
    {
        if (linkSegments.Length != 1)
        {
            return false;
        }

        var useNewsPrefixInLinks = await newsArticleSettingsService.GetUseNewsPrefixInLinksSettingAsync(storeId);

        if (useNewsPrefixInLinks)
        {
            return false;
        }

        return true;
    }
}
