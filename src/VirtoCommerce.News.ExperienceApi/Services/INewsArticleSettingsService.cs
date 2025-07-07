using System.Threading.Tasks;

namespace VirtoCommerce.News.ExperienceApi.Services;

public interface INewsArticleSettingsService
{
    Task<bool> GetUseNewsPrefixInLinksSettingAsync(string storeId);
}
