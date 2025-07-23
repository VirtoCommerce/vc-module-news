import { computed, ref } from "vue";
import * as _ from "lodash-es";
import { useAsync, useApiClient, useLoading } from "@vc-shell/framework";
import { NewsArticleClient, NewsArticle } from "../../../../api_client/virtocommerce.news";

export default () => {
  const { getApiClient: getNewsApiClient } = useApiClient(NewsArticleClient);

  const newsArticle = ref<NewsArticle>(new NewsArticle({ localizedContents: [], seoInfos: [] }));
  const originalNewsArticle = ref<NewsArticle>(new NewsArticle({ localizedContents: [], seoInfos: [] }));

  const resetNewsArticle = () => {
    if (originalNewsArticle.value) {
      newsArticle.value = _.cloneDeep(originalNewsArticle.value);
    }
  };

  const newsArticleIsDirty = computed(() => {
    const saveable = _.cloneDeep(newsArticle.value);
    cleanupEmptyLocalizations(saveable);
    cleanupEmptySeoInfos(saveable);

    return !_.isEqual(saveable, originalNewsArticle.value);
  });

  const newsArticleCanPublish = computed(
    () =>
      !newsArticleIsDirty.value &&
      originalNewsArticle.value &&
      !originalNewsArticle.value.isPublished &&
      hasContent(originalNewsArticle.value),
  );

  const newsArticleCanUnpublish = computed(
    () => !newsArticleIsDirty.value && originalNewsArticle.value && originalNewsArticle.value.isPublished,
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
      cleanupEmptyLocalizations(saveable);
      cleanupEmptySeoInfos(saveable);

      const apiClient = await getNewsApiClient();

      if (!saveable.id) {
        const createResult = await apiClient.create(saveable);
        newsArticle.value.id = createResult.id;
        saveable.id = createResult.id;
      } else {
        await apiClient.update(saveable);
      }
      originalNewsArticle.value = _.cloneDeep(saveable);
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

  const cleanupEmptyLocalizations = (newsArticleValue: NewsArticle) => {
    const notEmptyLocalizations = newsArticleValue.localizedContents?.filter(
      (x) => x.title || x.content || x.contentPreview,
    );
    newsArticleValue.localizedContents = notEmptyLocalizations;
  };

  const cleanupEmptySeoInfos = (newsArticleValue: NewsArticle) => {
    const notEmptySeoInfos = newsArticleValue.seoInfos?.filter(
      (x) =>
        x.isActive === true ||
        x.semanticUrl ||
        x.pageTitle ||
        x.metaKeywords ||
        x.metaDescription ||
        x.imageAltDescription,
    );
    newsArticleValue.seoInfos = notEmptySeoInfos;
  };

  const hasContent = (newsArticleValue: NewsArticle) => {
    const validLocalizations = newsArticleValue.localizedContents?.filter((x) => x.title || x.content);
    if (!validLocalizations) {
      return false;
    }

    return validLocalizations.length > 0;
  };

  return {
    newsArticle,
    loadNewsArticle,
    saveNewsArticle,
    loadingOrSavingNewsArticle: useLoading(
      loadingNewsArticle,
      savingNewsArticle,
      publishingNewsArticle,
      unpublishingNewsArticle,
    ),

    publishNewsArticle,
    unpublishNewsArticle,
    newsArticleCanPublish,
    newsArticleCanUnpublish,

    newsArticleIsDirty,
    resetNewsArticle,
  };
};
