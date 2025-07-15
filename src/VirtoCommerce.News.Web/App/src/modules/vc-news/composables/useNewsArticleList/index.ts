import { computed, ref } from "vue";
import { useAsync, useLoading, useApiClient, useListFactory } from "@vc-shell/framework";
import { NewsArticleClient, NewsArticle, NewsArticleSearchCriteria } from "../../../../api_client/virtocommerce.news";

export default () => {
  const pageSize = 20;

  const { getApiClient: getNewsApiClient } = useApiClient(NewsArticleClient);

  const searchQuery = ref<NewsArticleSearchCriteria>(new NewsArticleSearchCriteria());
  const newsArticles = ref<NewsArticle[]>([]);
  const newsArticlesCount = ref(0);
  const pagesCount = ref(0);
  const pageIndex = ref(1);

  const { loading: loadingNewsArticles, action: searchNewsArticles } = useAsync(async () => {
    const apiClient = await getNewsApiClient();
    const apiResult = await apiClient.search({
      ...(searchQuery.value ?? {}),
      take: pageSize,
      skip: pageSize * (pageIndex.value - 1),
    } as NewsArticleSearchCriteria);

    if (apiResult) {
      newsArticles.value = apiResult.results ?? [];
      newsArticlesCount.value = apiResult.totalCount ?? 0;
      pagesCount.value = Math.ceil(newsArticlesCount.value / pageSize);
    }
  });

  const { action: deleteNewsArticles } = useAsync<{ ids: string[] }>(async (args?: { ids: string[] }) => {
    if (args) {
      const apiClient = await getNewsApiClient();
      await apiClient.delete(args.ids);
    }
  });

  return {
    newsArticles,
    newsArticlesCount,
    pageSize,
    pagesCount,
    pageIndex,
    searchQuery,
    searchNewsArticles,
    loadingNewsArticles,

    deleteNewsArticles,
  };
};
