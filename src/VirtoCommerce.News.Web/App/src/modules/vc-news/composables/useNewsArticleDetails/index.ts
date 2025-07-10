import { computed, ref } from "vue";
import { useAsync, useLoading, useApiClient, useDetailsFactory } from "@vc-shell/framework";
import { NewsArticleClient, NewsArticle } from "../../../../api_client/virtocommerce.news";

export default () => {
  const { getApiClient } = useApiClient(NewsArticleClient);

  const item = ref<NewsArticle>(new NewsArticle());

  const { loading, action: get } = useAsync<{ id: string }>(async (args?: { id: string }) => {
    if (args) {
      const apiClient = await getApiClient();
      const apiResult = await apiClient.get(args.id);

      if (apiResult) {
        item.value = apiResult;
      }
    }
  });

  const { action: save } = useAsync<NewsArticle>(async (item?: NewsArticle) => {
    if (item) {
      const apiClient = await getApiClient();

      if (!item.id) {
        const apiResult = await apiClient.create(item);
      } else {
        const apiResult = await apiClient.update(item);
      }
    }
  });

  /*const factory = useDetailsFactory<NewsArticle>({
    load: async (itemId) => {
      if (itemId?.id) {
        const client = await getApiClient();
        return client.get(itemId.id);
      }
    },c
  });

  const { item, load, loading, saveChanges } = factory();
*/
  return {
    loading,
    item,
    get,
    save,
  };
};
