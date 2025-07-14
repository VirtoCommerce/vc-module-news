import { computed, ref } from "vue";
import { useAsync, useLoading, useApiClient, useLanguages } from "@vc-shell/framework";
import { NewsArticleClient, NewsArticle, NewsArticleLocalizedContent } from "../../../../api_client/virtocommerce.news";
import { StoreModuleClient, Store, StoreSearchCriteria } from "../../../../api_client/virtocommerce.store";
import { SettingClient } from "../../../../api_client/virtocommerce.platform";

interface LanguageOption {
  value: string;
  label: string;
  flag: string;
}

export default () => {
  const { getApiClient: getNewsApiClient } = useApiClient(NewsArticleClient);
  const { getApiClient: getStoreApiClient } = useApiClient(StoreModuleClient);
  const { getApiClient: getSettingApiClient } = useApiClient(SettingClient);

  const item = ref<NewsArticle>(new NewsArticle());
  const stores = ref<Store[]>([]);
  const userGroups = ref<string[]>([]);

  const { loading, action: get } = useAsync<{ id: string }>(async (args?: { id: string }) => {
    if (args) {
      const apiClient = await getNewsApiClient();
      const apiResult = await apiClient.get(args.id);

      if (apiResult) {
        item.value = apiResult;
      }
    }
  });

  const { action: getStores } = useAsync(async () => {
    const apiClient = await getStoreApiClient();
    const apiResult = await apiClient.searchStores(new StoreSearchCriteria());

    if (apiResult && apiResult.stores) {
      stores.value = apiResult.stores;
    }
  });

  const { action: getUserGroups } = useAsync(async () => {
    const apiClient = await getSettingApiClient();
    const apiResult = await apiClient.getGlobalSetting("Customer.MemberGroups");

    if (apiResult && apiResult.allowedValues) {
      userGroups.value = apiResult.allowedValues;
    }
  });

  const { action: save } = useAsync<NewsArticle>(async (item?: NewsArticle) => {
    if (item) {
      const apiClient = await getNewsApiClient();

      if (!item.id) {
        const apiResult = await apiClient.create(item);
      } else {
        const apiResult = await apiClient.update(item);
      }
    }
  });

  const { getLocaleByTag, getFlag } = useLanguages();

  const currentLocale = ref<string>("en-US");
  const setLocale = (locale: string) => {
    currentLocale.value = locale;
  };
  const languages = ref<LanguageOption[]>([]);

  const { loading: languagesLoading, action: getLanguages } = useAsync(async () => {
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

  const selectedLocalizedContent = computed(
    () =>
      item.value?.localizedContents?.find((x) => x.languageCode === currentLocale.value) ??
      new NewsArticleLocalizedContent(),
  );

  return {
    loading,
    item,
    stores,
    userGroups,
    get,
    getStores,
    getUserGroups,
    save,

    currentLocale,
    setLocale,
    languages,
    getLanguages,

    selectedLocalizedContent,
  };
};
