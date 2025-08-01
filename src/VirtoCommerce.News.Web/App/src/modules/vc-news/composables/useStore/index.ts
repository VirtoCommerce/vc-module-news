import { ref } from "vue";
import { useAsync, useApiClient } from "@vc-shell/framework";
import { StoreModuleClient, Store, StoreSearchCriteria } from "../../../../api_client/virtocommerce.store";

export default () => {
  const { getApiClient: getStoreApiClient } = useApiClient(StoreModuleClient);

  const stores = ref<Store[]>([]);

  const { loading: loadingStores, action: loadStores } = useAsync(async () => {
    const apiClient = await getStoreApiClient();
    const apiResult = await apiClient.searchStores(new StoreSearchCriteria());

    if (apiResult && apiResult.stores) {
      stores.value = apiResult.stores;
    }
  });

  return {
    stores,
    loadStores,
    loadingStores,
  };
};
