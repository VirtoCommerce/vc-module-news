import { computed, ref } from "vue";
import { useAsync, useLoading, useApiClient, useDetailsFactory } from "@vc-shell/framework";
import { NewsArticleClient, NewsArticle } from "../../../../api_client/virtocommerce.news";
import { StoreModuleClient, Store, StoreSearchCriteria } from "../../../../api_client/virtocommerce.store";
import { SettingClient } from "../../../../api_client/virtocommerce.platform";

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

    if (apiResult) {
      stores.value = apiResult.stores ?? [];
    }
  });

  const { action: getUserGroups } = useAsync(async () => {
    const apiClient = await getSettingApiClient();
    const apiResult = await apiClient.getGlobalSetting("Customer.MemberGroups");

    if (apiResult) {
      userGroups.value = apiResult.allowedValues ?? [];
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

  return {
    loading,
    item,
    stores,
    userGroups,
    get,
    getStores,
    getUserGroups,
    save,
  };
};
