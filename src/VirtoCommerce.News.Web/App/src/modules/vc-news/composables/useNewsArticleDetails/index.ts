import { computed, ref } from "vue";
import { useAsync, useApiClient } from "@vc-shell/framework";
import { NewsArticleClient, NewsArticle } from "../../../../api_client/virtocommerce.news";

export default () => {
  const { getApiClient: getNewsApiClient } = useApiClient(NewsArticleClient);

  const newsArticle = ref<NewsArticle>(new NewsArticle());

  const loadingOrSavingNewsArticle = computed(() => loadingNewsArticle.value || savingNewsArticle.value);

  const { loading: loadingNewsArticle, action: loadNewsArticle } = useAsync<{ id: string }>(
    async (args?: { id: string }) => {
      if (args) {
        const apiClient = await getNewsApiClient();
        const apiResult = await apiClient.get(args.id);

        if (apiResult) {
          newsArticle.value = apiResult;
        }
      }
    },
  );

  const { loading: savingNewsArticle, action: saveNewsArticle } = useAsync<NewsArticle>(async () => {
    if (newsArticle.value) {
      const apiClient = await getNewsApiClient();

      if (!newsArticle.value.id) {
        const apiResult = await apiClient.create(newsArticle.value);
      } else {
        const apiResult = await apiClient.update(newsArticle.value);
      }
    }
  });

  return {
    newsArticle,
    loadNewsArticle,
    saveNewsArticle,
    loadingOrSavingNewsArticle,
  };
};
