using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.News.Core.Models;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.News.ExperienceApi.Services;

public class NewsArticleLocalizedContentService
{
    public void FilterLanguages(IEnumerable<NewsArticle> newsArticles, string languageCode)
    {
        foreach (var newsArticle in newsArticles)
        {
            FilterLanguages(newsArticle, languageCode);
        }
    }

    public void FilterLanguages(NewsArticle newsArticle, string languageCode)
    {
        newsArticle.LocalizedContents = newsArticle.LocalizedContents
                .Where(lc => lc.LanguageCode.EqualsIgnoreCase(languageCode))
                .ToList();
    }
}
