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
    const saveable = getSaveable();

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

  const newsArticleCanArchive = computed(
    () => !newsArticleIsDirty.value && originalNewsArticle.value && !originalNewsArticle.value.isArchived,
  );

  const newsArticleCanUnarchive = computed(
    () => !newsArticleIsDirty.value && originalNewsArticle.value && originalNewsArticle.value.isArchived,
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

  const { loading: savingNewsArticle, action: saveNewsArticle } = useAsync(async () => {
    if (newsArticle.value) {
      const saveable = getSaveable();

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

  const { loading: publishingNewsArticle, action: publishNewsArticle } = useAsync(async () => {
    if (newsArticle.value) {
      const apiClient = await getNewsApiClient();

      if (newsArticle.value.id) {
        await apiClient.publish([newsArticle.value.id]);
      }
    }
  });

  const { loading: unpublishingNewsArticle, action: unpublishNewsArticle } = useAsync(async () => {
    if (newsArticle.value) {
      const apiClient = await getNewsApiClient();

      if (newsArticle.value.id) {
        await apiClient.unpublish([newsArticle.value.id]);
      }
    }
  });

  const { loading: archivingNewsArticle, action: archiveNewsArticle } = useAsync(async () => {
    if (newsArticle.value) {
      const apiClient = await getNewsApiClient();

      if (newsArticle.value.id) {
        await apiClient.archive([newsArticle.value.id]);
      }
    }
  });

  const { loading: unarchivingNewsArticle, action: unarchiveNewsArticle } = useAsync(async () => {
    if (newsArticle.value) {
      const apiClient = await getNewsApiClient();

      if (newsArticle.value.id) {
        await apiClient.unarchive([newsArticle.value.id]);
      }
    }
  });

  const { loading: cloningNewsArticle, action: cloneNewsArticle } = useAsync(async () => {
    if (newsArticle.value) {
      const saveable = getSaveable();

      const apiClient = await getNewsApiClient();

      const cloneResult = await apiClient.clone(saveable);

      newsArticle.value.id = cloneResult.id;
      saveable.id = cloneResult.id;

      originalNewsArticle.value = _.cloneDeep(saveable);
    }
  });

  const getSaveable = () => {
    const result = _.cloneDeep(newsArticle.value);

    cleanupEmptyLocalizations(result);
    cleanupEmptySeoInfos(result);

    return result;
  };

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
      archivingNewsArticle,
      unarchivingNewsArticle,
      cloningNewsArticle,
    ),

    publishNewsArticle,
    unpublishNewsArticle,
    newsArticleCanPublish,
    newsArticleCanUnpublish,

    archiveNewsArticle,
    unarchiveNewsArticle,
    newsArticleCanArchive,
    newsArticleCanUnarchive,

    cloneNewsArticle,

    newsArticleIsDirty,
    resetNewsArticle,
  };
};
