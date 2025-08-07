import { ref } from "vue";
import { useAsync, useApiClient } from "@vc-shell/framework";
import { NewsArticleClient, NewsArticle, NewsArticleSearchCriteria, NewsArticleSearchResult } from "../../../../api_client/virtocommerce.news";

export default () => {
  const pageSize = 20;

  const { getApiClient: getNewsApiClient } = useApiClient(NewsArticleClient);

  const searchQuery = ref<NewsArticleSearchCriteria>(new NewsArticleSearchCriteria());
  const newsArticles = ref<NewsArticle[]>([]);
  const newsArticlesCount = ref(0);
  const pagesCount = ref(0);
  const pageIndex = ref(1);

  const { loading: loadingNewsArticlesAll, action: searchNewsArticlesAll } = useAsync(async () => {
    const apiClient = await getNewsApiClient();
    await requestNewsArticles(apiClient.search); 
  });

  const { loading: loadingNewsArticlesPublished, action: searchNewsArticlesPublished } = useAsync(async () => {
    const apiClient = await getNewsApiClient();
    await requestNewsArticles(apiClient.search); 
  });

  const { loading: loadingNewsArticlesDrafts, action: searchNewsArticlesDrafts } = useAsync(async () => {
    const apiClient = await getNewsApiClient();
    await requestNewsArticles(apiClient.search); 
  });

  const { loading: loadingNewsArticlesScheduled, action: searchNewsArticlesScheduled } = useAsync(async () => {
    const apiClient = await getNewsApiClient();
    await requestNewsArticles(apiClient.search); 
  });

  const { loading: loadingNewsArticlesArchived, action: searchNewsArticlesArchived } = useAsync(async () => {
    const apiClient = await getNewsApiClient();
    await requestNewsArticles(apiClient.search); 
  });

  const requestNewsArticles = async (method: (searchCriteria: NewsArticleSearchCriteria) => Promise<NewsArticleSearchResult>) => {
    const apiResult = await method({
      ...(searchQuery.value ?? {}),
      take: pageSize,
      skip: pageSize * (pageIndex.value - 1),
    } as NewsArticleSearchCriteria);

    if (apiResult) {
      newsArticles.value = apiResult.results ?? [];
      newsArticlesCount.value = apiResult.totalCount ?? 0;
      pagesCount.value = Math.ceil(newsArticlesCount.value / pageSize);
    }
  };

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
    searchNewsArticlesAll,
    loadingNewsArticlesAll,
    searchNewsArticlesPublished,
    loadingNewsArticlesPublished,
    searchNewsArticlesDrafts,
    loadingNewsArticlesDrafts,
    searchNewsArticlesScheduled,
    loadingNewsArticlesScheduled,
    searchNewsArticlesArchived,
    loadingNewsArticlesArchived,

    deleteNewsArticles,
  };
};
