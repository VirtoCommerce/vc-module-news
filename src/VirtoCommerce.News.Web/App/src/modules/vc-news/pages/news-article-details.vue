<template>
  <VcBlade
    v-loading="loading"
    :title="title"
    :toolbar-items="bladeToolbar"
    :closable="closable"
    :expanded="expanded"
    width="60%"
    @close="$emit('close:blade')"
    @expand="$emit('expand:blade')"
    @collapse="$emit('collapse:blade')"
  >
    <div class="tw-absolute tw-top-2 tw-right-4 tw-z-10">
      <VcLanguageSelector
        v-if="props.param"
        :model-value="currentLocale"
        :options="languages"
        @update:model-value="setLocale"
      />
    </div>

    <VcContainer>
      <VcForm class="tw-flex tw-flex-col tw-gap-4">
        <Field
          v-slot="{ errors, errorMessage, handleChange }"
          :label="$t('VC_NEWS.PAGES.DETAILS.FORM.NAME.LABEL')"
          :model-value="newsArticle.name"
          name="name"
          rules="required"
        >
          <VcInput
            v-model="newsArticle.name"
            :label="$t('VC_NEWS.PAGES.DETAILS.FORM.NAME.LABEL')"
            required
            :max-length="1024"
            :error="errors.length > 0"
            :error-message="errorMessage"
            @update:model-value="handleChange"
          />
        </Field>

        <Field
          v-slot="{ errors, errorMessage, handleChange }"
          :label="$t('VC_NEWS.PAGES.DETAILS.FORM.STORE.LABEL')"
          :model-value="newsArticle.storeId"
          name="storeId"
          rules="required"
        >
          <VcSelect
            v-model="newsArticle.storeId"
            :label="$t('VC_NEWS.PAGES.DETAILS.FORM.STORE.LABEL')"
            :options="storeOptions"
            required
            :error="errors.length > 0"
            :error-message="errorMessage"
            @update:model-value="handleChange"
          />
        </Field>

        <VcCard
          :header="$t('VC_NEWS.PAGES.DETAILS.FORM.BLOCKS.PUBLISH')"
          is-collapsable
          class="tw-flex tw-flex-col tw-gap-4 tw-p-4"
        >
          <div class="tw-flex tw-flex-row tw-gap-4">
            <VcInput
              v-model="newsArticle.publishDate"
              type="datetime-local"
              :label="$t('VC_NEWS.PAGES.DETAILS.FORM.PUBLISH_DATE.LABEL')"
            />

            <VcInput
              v-model="newsArticle.archiveDate"
              type="datetime-local"
              :label="$t('VC_NEWS.PAGES.DETAILS.FORM.ARCHIVE_DATE.LABEL')"
            />
          </div>

          <Field
            v-slot="{ errors, errorMessage, handleChange }"
            :label="$t('VC_NEWS.PAGES.DETAILS.FORM.PUBLISH_SCOPE.LABEL')"
            :model-value="newsArticle.publishScope"
            name="publishScope"
            rules="required"
          >
            <VcSelect
              v-model="newsArticle.publishScope"
              :label="$t('VC_NEWS.PAGES.DETAILS.FORM.PUBLISH_SCOPE.LABEL')"
              :options="publishScopeOptions"
              required
              :error="errors.length > 0"
              :error-message="errorMessage"
              @update:model-value="handleChange"
            />
          </Field>

          <VcMultivalue
            v-if="newsArticle.publishScope === 'Authorized'"
            v-model="userGroupsSelected"
            :label="$t('VC_NEWS.PAGES.DETAILS.FORM.USER_GROUPS.LABEL')"
            :options="userGroupsOptions"
            option-value="id"
            option-label="title"
            multivalue
            class="tw-flex-auto"
          />
        </VcCard>

        <VcCard
          v-if="props.param"
          :header="$t('VC_NEWS.PAGES.DETAILS.FORM.BLOCKS.CONTENT')"
          is-collapsable
          class="tw-flex tw-flex-col tw-gap-4 tw-p-4"
        >
          <Field
            v-slot="{ errors, errorMessage, handleChange }"
            :label="$t('VC_NEWS.PAGES.DETAILS.FORM.CONTENT_TITLE.LABEL')"
            :model-value="selectedLocalizedContent.title"
            name="content-title"
            :rules="{ required: !!selectedLocalizedContent.content || !!selectedLocalizedContent.contentPreview }"
          >
            <VcInput
              v-model="selectedLocalizedContent.title"
              :label="$t('VC_NEWS.PAGES.DETAILS.FORM.CONTENT_TITLE.LABEL')"
              :required="!!selectedLocalizedContent.content || !!selectedLocalizedContent.contentPreview"
              :error="errors.length > 0"
              :error-message="errorMessage"
              multilanguage
              :current-language="currentLocale"
              @update:model-value="handleChange"
            />
          </Field>

          <VcEditor
            v-model="selectedLocalizedContent.contentPreview"
            :label="$t('VC_NEWS.PAGES.DETAILS.FORM.CONTENT_PREVIEW.LABEL')"
            multilanguage
            :current-language="currentLocale"
            assets-folder="news-articles"
          />

          <Field
            v-slot="{ errors, errorMessage, handleChange }"
            :label="$t('VC_NEWS.PAGES.DETAILS.FORM.CONTENT_CONTENT.LABEL')"
            :model-value="selectedLocalizedContent.content"
            name="content-content"
            :rules="{ required: !!selectedLocalizedContent.title || !!selectedLocalizedContent.contentPreview }"
          >
            <VcEditor
              v-model="selectedLocalizedContent.content"
              :label="$t('VC_NEWS.PAGES.DETAILS.FORM.CONTENT_CONTENT.LABEL')"
              :required="!!selectedLocalizedContent.title || !!selectedLocalizedContent.contentPreview"
              :error="errors.length > 0"
              :error-message="errorMessage"
              multilanguage
              :current-language="currentLocale"
              assets-folder="news-articles"
              @update:model-value="handleChange"
            />
          </Field>
        </VcCard>

        <VcCard
          v-if="props.param"
          :header="$t('VC_NEWS.PAGES.DETAILS.FORM.BLOCKS.METADATA')"
          is-collapsable
          is-collapsed
          class="tw-flex tw-flex-col tw-gap-4 tw-p-4"
        >
          <VcSelect
            v-model="newsArticle.authorId"
            :label="$t('VC_NEWS.PAGES.DETAILS.FORM.AUTHOR.LABEL')"
            :options="authorOptions"
          />

          <VcMultivalue
            v-model="tagsSelected"
            :label="$t('VC_NEWS.PAGES.DETAILS.FORM.TAGS.LABEL')"
            :options="tagsOptions"
            option-value="id"
            option-label="title"
            :multivalue="false"
            multilanguage
            :current-language="currentLocale"
          />
        </VcCard>

        <VcCard
          v-if="props.param"
          :header="$t('VC_NEWS.PAGES.DETAILS.FORM.BLOCKS.SEO')"
          is-collapsable
          is-collapsed
          class="tw-flex tw-flex-col tw-gap-4 tw-p-4"
        >
          <VcSwitch
            v-model="selectedSeo.isActive"
            :label="$t('VC_NEWS.PAGES.DETAILS.FORM.SEO_IS_ACTIVE.LABEL')"
          />

          <Field
            v-slot="{ errors, errorMessage, handleChange }"
            :label="$t('VC_NEWS.PAGES.DETAILS.FORM.SEO_SEMANTIC_URL.LABEL')"
            :model-value="selectedSeo.semanticUrl"
            name="seo-url"
            :rules="{
              required:
                selectedSeo.isActive === true ||
                !!selectedSeo.pageTitle ||
                !!selectedSeo.metaDescription ||
                !!selectedSeo.metaKeywords ||
                !!selectedSeo.imageAltDescription,
            }"
          >
            <VcInput
              v-model="selectedSeo.semanticUrl"
              :label="$t('VC_NEWS.PAGES.DETAILS.FORM.SEO_SEMANTIC_URL.LABEL')"
              :required="
                selectedSeo.isActive === true ||
                !!selectedSeo.pageTitle ||
                !!selectedSeo.metaDescription ||
                !!selectedSeo.metaKeywords ||
                !!selectedSeo.imageAltDescription
              "
              :error="errors.length > 0"
              :error-message="errorMessage"
              multilanguage
              :current-language="currentLocale"
              @update:model-value="handleChange"
            />
          </Field>

          <VcInput
            v-model="selectedSeo.pageTitle"
            :label="$t('VC_NEWS.PAGES.DETAILS.FORM.SEO_PAGE_TITLE.LABEL')"
            multilanguage
            :current-language="currentLocale"
          />

          <VcTextarea
            v-model="selectedSeo.metaDescription"
            :label="$t('VC_NEWS.PAGES.DETAILS.FORM.SEO_META_DESCRIPTION.LABEL')"
            multilanguage
            :current-language="currentLocale"
          />

          <VcInput
            v-model="selectedSeo.metaKeywords"
            :label="$t('VC_NEWS.PAGES.DETAILS.FORM.SEO_META_KEYWORDS.LABEL')"
            multilanguage
            :current-language="currentLocale"
          />

          <VcInput
            v-model="selectedSeo.imageAltDescription"
            :label="$t('VC_NEWS.PAGES.DETAILS.FORM.SEO_IMAGE_ALT_TEXT.LABEL')"
            multilanguage
            :current-language="currentLocale"
          />
        </VcCard>
      </VcForm>
    </VcContainer>
  </VcBlade>
</template>

<script lang="ts" setup>
import { onMounted, ref, Ref, computed } from "vue";
import { useI18n } from "vue-i18n";
import { Field, useForm } from "vee-validate";
import {
  IBladeToolbar,
  IParentCallArgs,
  VcLanguageSelector,
  usePermissions,
  useBladeNavigation,
  usePopup,
  useLoading,
} from "@vc-shell/framework";
import {
  useNewsArticleDetails,
  useNewsArticlePermissions,
  useStore,
  useUserGroups,
  useLocalization,
  useCustomers,
} from "../composables";
import { NewsArticleLocalizedContent, NewsArticleLocalizedTag, SeoInfo } from "../../../api_client/virtocommerce.news";

export interface Emits {
  (event: "parent:call", args: IParentCallArgs): void;
  (event: "collapse:blade"): void;
  (event: "expand:blade"): void;
  (event: "close:blade"): void;
}

const emit = defineEmits<Emits>();

export interface Props {
  expanded?: boolean;
  closable?: boolean;
  param?: string;
}

const props = withDefaults(defineProps<Props>(), {
  expanded: true,
  closable: true,
  param: undefined,
});

defineOptions({
  url: "/details",
  name: "NewsArticleDetails",
});

const { t } = useI18n({ useScope: "global" });

const { onBeforeClose } = useBladeNavigation();
const { hasAccess } = usePermissions();
const { showConfirmation } = usePopup();

const { meta } = useForm({ validateOnMount: false });

//stores
const { stores, loadStores, loadingStores } = useStore();
const storeOptions = computed(() => stores.value.map((x) => ({ id: x.id, title: x.name })));

//userGroups
const { userGroups, loadUserGroups, loadingUserGroups } = useUserGroups();
const userGroupsOptions = computed(() => userGroups.value.map((x) => ({ id: x, title: x })));

const userGroupsSelected = computed({
  get() {
    return newsArticle.value?.userGroups?.map((x) => ({ id: x, title: x })) ?? [];
  },
  set(newValue) {
    newsArticle.value!.userGroups = newValue.map((x) => x.id);
  },
});

//tags
const tagsOptions = computed(() => newsArticleOptions.value?.tags?.map((x) => ({ id: x, title: x })));

const tagsSelected = computed({
  get() {
    return newsArticle.value?.localizedTags
      ?.filter((x) => x.languageCode === currentLocale.value)
      .map((x) => ({ id: x.tag, title: x.tag }));
  },
  set(newValue) {
    const newTags = newValue?.map(
      (x) => new NewsArticleLocalizedTag({ tag: x.title, languageCode: currentLocale.value }),
    );
    newsArticle.value.localizedTags =
      newsArticle.value.localizedTags?.filter((x) => x.languageCode !== currentLocale.value) ?? [];
    if (newTags) {
      newsArticle.value.localizedTags.push(...newTags);
    }
  },
});

//publish scopes
const publishScopeOptions = computed(() =>
  newsArticleOptions.value?.publishScopes?.map((x) => ({
    id: x,
    title: t(`VC_NEWS.PAGES.DETAILS.FORM.PUBLISH_SCOPE.VALUES.${x}`),
  })),
);

//localization
const { languages, loadLanguages, loadingLanguages } = useLocalization();

const currentLocale = ref<string>("en-US");
const setLocale = async (locale: string) => {
  currentLocale.value = locale;
  await loadOptions({ languageCode: currentLocale.value });
};

//authors
const { authors, loadAuthors, loadingAuthors } = useCustomers();
const authorOptions = computed(() => authors.value.map((x) => ({ id: x.id, title: x.name })));

//content
const selectedLocalizedContent = computed(() => {
  if (newsArticle.value) {
    if (!newsArticle.value.localizedContents) {
      newsArticle.value.localizedContents = [];
    }
  }
  const existingLocalizedContent = newsArticle.value?.localizedContents?.find(
    (x) => x.languageCode === currentLocale.value,
  );

  if (existingLocalizedContent) {
    return existingLocalizedContent;
  }

  const newLocalizedContent = new NewsArticleLocalizedContent();
  newLocalizedContent.languageCode = currentLocale.value;
  newsArticle.value?.localizedContents?.push(newLocalizedContent);
  return newLocalizedContent;
});

//seo
const selectedSeo = computed(() => {
  if (newsArticle.value) {
    if (!newsArticle.value.seoInfos) {
      newsArticle.value.seoInfos = [];
    }
  }
  const existingSeoInfo = newsArticle.value?.seoInfos?.find((x) => x.languageCode === currentLocale.value);

  if (existingSeoInfo) {
    return existingSeoInfo;
  }

  const newSeoInfo = new SeoInfo();
  newSeoInfo.isActive = false;
  newSeoInfo.languageCode = currentLocale.value;
  newSeoInfo.storeId = newsArticle.value.storeId;
  newsArticle.value?.seoInfos?.push(newSeoInfo);
  return newSeoInfo;
});

//news article
const {
  newsArticle,
  newsArticleOptions,

  loadNewsArticle,
  saveNewsArticle,
  loadingOrSavingNewsArticle,

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

  loadOptions,
} = useNewsArticleDetails();
const { publishNewsArticlePermission, createNewsArticlePermission, updateNewsArticlePermission } =
  useNewsArticlePermissions();

const saveNewsArticlePermission = props.param ? updateNewsArticlePermission : createNewsArticlePermission;

//other
const loading = useLoading(
  loadingStores,
  loadingUserGroups,
  loadingLanguages,
  loadingAuthors,
  loadingOrSavingNewsArticle,
);

const bladeToolbar = ref([]) as Ref<IBladeToolbar[]>;

bladeToolbar.value.push({
  id: "save",
  icon: "material-save",
  title: computed(() => t("VC_NEWS.PAGES.DETAILS.TOOLBAR.SAVE")),
  disabled: computed(() => !meta.value.valid || !newsArticleIsDirty?.value),
  clickHandler: async () => {
    try {
      await saveNewsArticle();
      emit("parent:call", { method: "reload" });
      emit("parent:call", { method: "reOpenDetailsBlade", args: newsArticle.value!.id });
    } catch (error) {
      console.error("Failed to save news article:", error);
    }
  },
  isVisible: computed(() => hasAccess(saveNewsArticlePermission)),
});

if (props.param) {
  bladeToolbar.value.push({
    id: "reset",
    icon: "material-undo",
    title: computed(() => t("VC_NEWS.PAGES.DETAILS.TOOLBAR.RESET")),
    disabled: computed(() => !newsArticleIsDirty?.value),
    clickHandler: async () => {
      resetNewsArticle();
    },
  });
  bladeToolbar.value.push({
    id: "clone",
    icon: "material-content_copy",
    title: computed(() => t("VC_NEWS.PAGES.DETAILS.TOOLBAR.CLONE")),
    disabled: computed(() => newsArticleIsDirty?.value),
    clickHandler: async () => {
      await cloneNewsArticle();
      emit("parent:call", { method: "reload" });
      emit("parent:call", { method: "reOpenDetailsBlade", args: newsArticle.value!.id });
    },
    isVisible: computed(() => hasAccess(createNewsArticlePermission)),
  });

  bladeToolbar.value.push({
    id: "publish",
    icon: "material-visibility",
    title: computed(() => t("VC_NEWS.PAGES.DETAILS.TOOLBAR.PUBLISH")),
    clickHandler: async () => {
      await publishNewsArticle();
      emit("parent:call", { method: "reload" });
      emit("parent:call", { method: "reOpenDetailsBlade", args: newsArticle.value!.id });
    },
    isVisible: computed(() => hasAccess(publishNewsArticlePermission) && newsArticleCanPublish?.value),
  });
  bladeToolbar.value.push({
    id: "unpublish",
    icon: "material-visibility_off",
    title: computed(() => t("VC_NEWS.PAGES.DETAILS.TOOLBAR.UNPUBLISH")),
    clickHandler: async () => {
      await unpublishNewsArticle();
      emit("parent:call", { method: "reload" });
      emit("parent:call", { method: "reOpenDetailsBlade", args: newsArticle.value!.id });
    },
    isVisible: computed(() => hasAccess(publishNewsArticlePermission) && newsArticleCanUnpublish?.value),
  });

  bladeToolbar.value.push({
    id: "archive",
    icon: "material-archive",
    title: computed(() => t("VC_NEWS.PAGES.DETAILS.TOOLBAR.ARCHIVE")),
    clickHandler: async () => {
      await archiveNewsArticle();
      emit("parent:call", { method: "reload" });
      emit("parent:call", { method: "reOpenDetailsBlade", args: newsArticle.value!.id });
    },
    isVisible: computed(() => hasAccess(publishNewsArticlePermission) && newsArticleCanArchive?.value),
  });
  bladeToolbar.value.push({
    id: "unarchive",
    icon: "material-unarchive",
    title: computed(() => t("VC_NEWS.PAGES.DETAILS.TOOLBAR.UNARCHIVE")),
    clickHandler: async () => {
      await unarchiveNewsArticle();
      emit("parent:call", { method: "reload" });
      emit("parent:call", { method: "reOpenDetailsBlade", args: newsArticle.value!.id });
    },
    isVisible: computed(() => hasAccess(publishNewsArticlePermission) && newsArticleCanUnarchive?.value),
  });
}

const title = computed(() => t("VC_NEWS.PAGES.DETAILS.TITLE"));

onMounted(async () => {
  await loadStores();
  await loadUserGroups();
  await loadLanguages();
  await loadAuthors();
  await loadOptions({ languageCode: currentLocale.value });
  if (props.param) {
    await loadNewsArticle({ id: props.param });
  }
});

onBeforeClose(async () => {
  if (newsArticleIsDirty.value) {
    const confirmed = await showConfirmation(t("VC_NEWS.PAGES.DETAILS.ALERTS.CLOSE_CONFIRMATION"));
    return confirmed;
  }
  return true;
});

defineExpose({
  title,
});
</script>
