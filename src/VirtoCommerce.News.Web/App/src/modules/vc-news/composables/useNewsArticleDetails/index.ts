import { computed, ref } from "vue";
import * as _ from "lodash-es";
import { useAsync, useApiClient } from "@vc-shell/framework";
import { NewsArticleClient, NewsArticle } from "../../../../api_client/virtocommerce.news";

export default () => {
  const { getApiClient: getNewsApiClient } = useApiClient(NewsArticleClient);

  const newsArticle = ref<NewsArticle>(new NewsArticle());
  const originalNewsArticle = ref<NewsArticle>();

  const resetNewsArticle = () => {
    if (originalNewsArticle.value) {
      newsArticle.value = _.cloneDeep(originalNewsArticle.value);
    }
  };

  const newsArticleIsDirty = computed(() => !_.isEqual(newsArticle.value, originalNewsArticle));

  const newsArticleCanPublish = computed(() => originalNewsArticle.value && !originalNewsArticle.value.isPublished);

  const newsArticleCanUnpublish = computed(() => originalNewsArticle.value && originalNewsArticle.value.isPublished);

  const loadingOrSavingNewsArticle = computed(
    () =>
      loadingNewsArticle.value ||
      savingNewsArticle.value ||
      publishingNewsArticle.value ||
      unpublishingNewsArticle.value,
  );

  const { loading: loadingNewsArticle, action: loadNewsArticle } = useAsync<{ id: string }>(
    async (args?: { id: string }) => {
      if (args) {
        const apiClient = await getNewsApiClient();
        const apiResult = await apiClient.get(args.id);

        if (apiResult) {
          newsArticle.value = apiResult;
          originalNewsArticle.value = _.cloneDeep(apiResult);
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

  const { loading: publishingNewsArticle, action: publishNewsArticle } = useAsync<NewsArticle>(async () => {
    if (newsArticle.value) {
      const apiClient = await getNewsApiClient();

      if (newsArticle.value.id) {
        await apiClient.publish([newsArticle.value.id]);
      }
    }
  });

  const { loading: unpublishingNewsArticle, action: unpublishNewsArticle } = useAsync<NewsArticle>(async () => {
    if (newsArticle.value) {
      const apiClient = await getNewsApiClient();

      if (newsArticle.value.id) {
        await apiClient.unpublish([newsArticle.value.id]);
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

    publishNewsArticle,
    unpublishNewsArticle,
    newsArticleCanPublish,
    newsArticleCanUnpublish,

    newsArticleIsDirty,
    resetNewsArticle,
  };
};
