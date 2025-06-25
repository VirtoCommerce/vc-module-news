using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.News.Core.Models;

namespace VirtoCommerce.News.ExperienceApi.Services;

public interface INewsArticleSeoService
{
    Task FilterSeoInfosAsync(IList<NewsArticle> newsArticles, string languageCode, string storeId);
}
