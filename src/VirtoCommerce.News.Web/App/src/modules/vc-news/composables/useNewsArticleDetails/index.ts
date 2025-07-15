import { computed, ref } from "vue";
import * as _ from "lodash-es";
import { useAsync, useApiClient } from "@vc-shell/framework";
import { NewsArticleClient, NewsArticle } from "../../../../api_client/virtocommerce.news";

export default () => {
  const { getApiClient: getNewsApiClient } = useApiClient(NewsArticleClient);

  const newsArticle = ref<NewsArticle>(new NewsArticle());
  let originalNewsArticle: NewsArticle;

  const resetNewsArticle = () => {
    newsArticle.value = _.cloneDeep(originalNewsArticle);
  };

  const newsArticleIsDirty = computed(() => !_.isEqual(newsArticle.value, originalNewsArticle));

  const loadingOrSavingNewsArticle = computed(() => loadingNewsArticle.value || savingNewsArticle.value);

  const { loading: loadingNewsArticle, action: loadNewsArticle } = useAsync<{ id: string }>(
    async (args?: { id: string }) => {
      if (args) {
        const apiClient = await getNewsApiClient();
        const apiResult = await apiClient.get(args.id);

        if (apiResult) {
          newsArticle.value = apiResult;
          originalNewsArticle = _.cloneDeep(apiResult);
        }
      }
    },
  );

  const { loading: savingNewsArticle, action: saveNewsArticle } = useAsync<NewsArticle>(async () => {
    if (newsArticle.value) {
      const saveable = _.cloneDeep(newsArticle.value);
      cleanupLocalizations(saveable);

      const apiClient = await getNewsApiClient();

      if (!saveable.id) {
        const createResult = await apiClient.create(saveable);
        newsArticle.value.id = createResult.id;
      } else {
        const updateResult = await apiClient.update(saveable);
      }
    }
  });

  const cleanupLocalizations = (newsArticle: NewsArticle) => {
    const notEmptyLocalizations = newsArticle.localizedContents?.filter(
      (x) => x.title || x.content || x.contentPreview,
    );
    newsArticle.localizedContents = notEmptyLocalizations;
  };

  return {
    newsArticle,
    loadNewsArticle,
    saveNewsArticle,
    loadingOrSavingNewsArticle,

    newsArticleIsDirty,
    resetNewsArticle,
  };
};
