using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.News.Core.Models;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.StoreModule.Core.Services;

namespace VirtoCommerce.News.ExperienceApi.Services;

public class NewsArticleLocalizationService(IStoreService storeService)
{
    public async Task FilterLanguagesAsync(IEnumerable<NewsArticle> newsArticles, string languageCode, string storeId)
    {
        string storeDefaultLanguage = null;
        if (!storeId.IsNullOrEmpty())
        {
            var store = await storeService.GetByIdAsync(storeId, null, false);
            storeDefaultLanguage = store?.DefaultLanguage;
        }

        foreach (var newsArticle in newsArticles)
        {
            var allLocalizedContents = newsArticle.LocalizedContents;
            newsArticle.LocalizedContents = allLocalizedContents
                .Where(lc => lc.LanguageCode.EqualsIgnoreCase(languageCode))
                .ToList();

            if (!newsArticle.LocalizedContents.Any())
            {
                newsArticle.LocalizedContents = allLocalizedContents
                    .Where(lc => lc.LanguageCode.EqualsIgnoreCase(storeDefaultLanguage))
                    .ToList();
            }
        }
    }
}
