import { computed, ref } from "vue";
import { useAsync, useLoading, useApiClient, useListFactory } from "@vc-shell/framework";
import { NewsArticleClient, NewsArticle, NewsArticleSearchCriteria } from "../../../../api_client/virtocommerce.news";

export default () => {
  const pageSize = 20;

  const { getApiClient } = useApiClient(NewsArticleClient);

  const searchQuery = ref<NewsArticleSearchCriteria>(new NewsArticleSearchCriteria());
  const items = ref<NewsArticle[]>([]);
  const itemsCount = ref(0);
  const pagesCount = ref(0);
  const pageIndex = ref(1);

  const { loading, action: search } = useAsync(async () => {
    const apiClient = await getApiClient();
    const apiResult = await apiClient.search({
      ...(searchQuery.value ?? {}),
      take: pageSize,
      skip: pageSize * (pageIndex.value - 1),
    } as NewsArticleSearchCriteria);

    if (apiResult) {
      items.value = apiResult.results ?? [];
      itemsCount.value = apiResult.totalCount ?? 0;
      pagesCount.value = Math.ceil(itemsCount.value / pageSize);
    }
  });

  const { action: deleteItems } = useAsync<{ ids: string[] }>(async (args?: { ids: string[] }) => {
    if (args) {
      const apiClient = await getApiClient();
      await apiClient.delete(args.ids);
    }
  });

  return {
    items,
    itemsCount,
    pageSize,
    pagesCount,
    pageIndex,
    search,
    searchQuery,
    loading,

    deleteItems,
  };
};
