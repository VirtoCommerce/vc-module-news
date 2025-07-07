using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Settings;
using NewsSettings = VirtoCommerce.News.Core.ModuleConstants.Settings.General;

namespace VirtoCommerce.News.ExperienceApi.Services;

public class NewsArticleSettingsService(ISettingsManager settingsManager) : INewsArticleSettingsService
{
    public async Task<bool> GetUseRootLinkSettingAsync()
    {
        return await settingsManager.GetValueAsync<bool>(NewsSettings.UseRootLinks);
    }
}
