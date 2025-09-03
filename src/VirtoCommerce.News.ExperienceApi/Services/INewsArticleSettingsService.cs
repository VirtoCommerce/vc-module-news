using System.Threading.Tasks;
using VirtoCommerce.News.ExperienceApi.Models;

namespace VirtoCommerce.News.ExperienceApi.Services;

public interface INewsArticleSettingsService
{
    Task<NewsArticleSettings> GetSettingsAsync(string storeId);
}
