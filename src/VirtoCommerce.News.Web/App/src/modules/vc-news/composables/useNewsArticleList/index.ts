import { computed, ref } from "vue";
import { useAsync, useLoading, useApiClient, useListFactory } from "@vc-shell/framework";
import { NewsArticleClient, NewsArticle, NewsArticleSearchCriteria } from "../../../../api_client/virtocommerce.news";

export default () => {
  const { getApiClient } = useApiClient(NewsArticleClient);

  const factory = useListFactory<NewsArticle[], NewsArticleSearchCriteria>({
    load: async (searchQuery) => {
      const client = await getApiClient();
      return client.search({
        take: 20,
        ...(searchQuery || {}),
      } as NewsArticleSearchCriteria);
    },
  });

  const { action: deleteItems } = useAsync<{ ids: string[] }>(async (args?: { ids: string[] }) => {
    if (args) {
      const apiClient = await getApiClient();
      await apiClient.delete(args.ids);
    }
  });

  const { load, items, pagination, loading, query } = factory();

  return {
    items,
    load,
    loading,
    pagination,
    query,
    deleteItems,
  };
};
