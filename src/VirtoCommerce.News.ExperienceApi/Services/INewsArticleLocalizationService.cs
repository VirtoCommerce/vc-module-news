using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.News.Core.Models;

namespace VirtoCommerce.News.ExperienceApi.Services;

public interface INewsArticleLocalizationService
{
    Task FilterLanguagesAsync(IList<NewsArticle> newsArticles, string languageCode, string storeId);
}
