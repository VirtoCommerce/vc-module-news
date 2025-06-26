using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.News.Core.Models;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.StoreModule.Core.Services;

namespace VirtoCommerce.News.ExperienceApi.Services;

public class NewsArticleLocalizationService(IStoreService storeService) : INewsArticleLocalizationService
{
    public virtual async Task FilterLanguagesAsync(IList<NewsArticle> newsArticles, string languageCode, string storeId)
    {
        string storeDefaultLanguage = null;

        if (!storeId.IsNullOrEmpty())
        {
            var store = await storeService.GetNoCloneAsync(storeId);
            storeDefaultLanguage = store?.DefaultLanguage;
        }

        foreach (var newsArticle in newsArticles)
        {
            var allLocalizedContents = newsArticle.LocalizedContents;

            newsArticle.LocalizedContents = allLocalizedContents
                .Where(x => x.LanguageCode.EqualsIgnoreCase(languageCode))
                .ToList();

            if (newsArticle.LocalizedContents.Count == 0)
            {
                newsArticle.LocalizedContents = allLocalizedContents
                    .Where(x => x.LanguageCode.EqualsIgnoreCase(storeDefaultLanguage))
                    .ToList();
            }
        }
    }
}
