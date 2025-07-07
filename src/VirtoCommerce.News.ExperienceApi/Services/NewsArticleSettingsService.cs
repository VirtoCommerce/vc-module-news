using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.StoreModule.Core.Services;
using NewsSettings = VirtoCommerce.News.Core.ModuleConstants.Settings.General;

namespace VirtoCommerce.News.ExperienceApi.Services;

public class NewsArticleSettingsService(IStoreService storeService, ISettingsManager settingsManager) : INewsArticleSettingsService
{
    public async Task<bool> GetUseRootLinkSettingAsync(string storeId)
    {
        var store = !storeId.IsNullOrEmpty() ? await storeService.GetNoCloneAsync(storeId) : null;

        if (store == null)
        {
            return await settingsManager.GetValueAsync<bool>(NewsSettings.UseRootLinks);
        }

        return store.Settings.GetValue<bool>(NewsSettings.UseRootLinks);
    }
}
