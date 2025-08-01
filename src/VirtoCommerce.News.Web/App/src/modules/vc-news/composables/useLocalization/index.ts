import { ref } from "vue";
import { useAsync, useApiClient, useLanguages } from "@vc-shell/framework";
import { SettingClient } from "../../../../api_client/virtocommerce.platform";

interface LanguageOption {
  value: string;
  label: string;
  flag: string;
}

export default () => {
  const { getApiClient: getSettingApiClient } = useApiClient(SettingClient);

  const { getLocaleByTag, getFlag } = useLanguages();

  const languages = ref<LanguageOption[]>([]);

  const { loading: loadingLanguages, action: loadLanguages } = useAsync(async () => {
    const apiClient = await getSettingApiClient();
    const apiResult = await apiClient.getGlobalSetting("VirtoCommerce.Core.General.Languages");

    if (apiResult && apiResult.allowedValues) {
      languages.value = await Promise.all(
        apiResult.allowedValues.map(async (x) => ({
          label: getLocaleByTag(x) || x,
          value: x,
          flag: await getFlag(x ?? ""),
        })),
      );
    }
  });

  return {
    languages,
    loadLanguages,
    loadingLanguages,
  };
};
