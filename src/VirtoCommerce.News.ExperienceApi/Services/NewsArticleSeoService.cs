using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.News.Core.Models;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Seo.Core.Extensions;
using VirtoCommerce.Seo.Core.Models;
using VirtoCommerce.StoreModule.Core.Services;

namespace VirtoCommerce.News.ExperienceApi.Services;

public class NewsArticleSeoService(IStoreService storeService) : INewsArticleSeoService
{
    public virtual async Task FilterSeoInfosAsync(IList<NewsArticle> newsArticles, string languageCode, string storeId)
    {
        string storeDefaultLanguage = null;
        if (!storeId.IsNullOrEmpty())
        {
            var store = await storeService.GetByIdAsync(storeId, null, false);
            storeDefaultLanguage = store?.DefaultLanguage;
        }

        foreach (var newsArticle in newsArticles)
        {
            SeoInfo seoInfo = null;

            if (!newsArticle.SeoInfos.IsNullOrEmpty())
            {
                seoInfo = newsArticle.SeoInfos.GetBestMatchingSeoInfo(storeId, storeDefaultLanguage, languageCode);
            }

            if (seoInfo == null)
            {
                seoInfo = SeoExtensions.GetFallbackSeoInfo(newsArticle.Id, newsArticle.Name, languageCode);
            }

            newsArticle.SeoInfos.Clear();
            newsArticle.SeoInfos.Add(seoInfo);
        }
    }
}
