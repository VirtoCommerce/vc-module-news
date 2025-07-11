using System.Threading.Tasks;
using VirtoCommerce.News.ExperienceApi.Model;

namespace VirtoCommerce.News.ExperienceApi.Services;

public interface INewsArticleSettingsService
{
    Task<NewsArticleSettings> GetSettingsAsync(string storeId);
}
