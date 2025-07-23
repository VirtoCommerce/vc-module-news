using System.Threading.Tasks;
using VirtoCommerce.News.ExperienceApi.Model;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.StoreModule.Core.Services;
using NewsSettings = VirtoCommerce.News.Core.ModuleConstants.Settings.General;

namespace VirtoCommerce.News.ExperienceApi.Services;

public class NewsArticleSettingsService(IStoreService storeService, ISettingsManager settingsManager) : INewsArticleSettingsService
{
    public async Task<NewsArticleSettings> GetSettingsAsync(string storeId)
    {
        var result = AbstractTypeFactory<NewsArticleSettings>.TryCreateInstance();

        var store = !storeId.IsNullOrEmpty() ? await storeService.GetNoCloneAsync(storeId) : null;

        if (store != null)
        {
            result.StoreDefaultLanguage = store.DefaultLanguage;
            result.UseNewsPrefixInLinks = store.Settings.GetValue<bool>(NewsSettings.UseNewsPrefixInLinks);
            result.UseStoreDefaultLanguage = store.Settings.GetValue<bool>(NewsSettings.UseStoreDefaultLanguage);
        }
        else
        {
            result.UseNewsPrefixInLinks = await settingsManager.GetValueAsync<bool>(NewsSettings.UseNewsPrefixInLinks);
            result.UseStoreDefaultLanguage = await settingsManager.GetValueAsync<bool>(NewsSettings.UseStoreDefaultLanguage);
        }

        return result;
    }
}
