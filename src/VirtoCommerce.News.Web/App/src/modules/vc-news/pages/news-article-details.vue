<template>
  <VcBlade
    :title="title"
    :toolbar-items="bladeToolbar"
    :closable="closable" :expanded="expanded"
    v-loading="loading"
    width="60%"
    @close="$emit('close:blade')" @expand="$emit('expand:blade')" @collapse="$emit('collapse:blade')">

    <div
      class="tw-absolute tw-top-2 tw-right-4 tw-z-10">
      <VcLanguageSelector
        :model-value="currentLocale"
        :options="languages"
        @update:model-value="setLocale" />
    </div>

    <VcContainer>
      <VcForm class="tw-flex tw-flex-col tw-gap-4">
        <Field
          :label="$t('VC_NEWS.PAGES.DETAILS.FORM.NAME.LABEL')"
          :model-value="newsArticle.name"
          name="name"
          rules="required"
          v-slot="{ errors, errorMessage, handleChange }">
          <VcInput
            :label="$t('VC_NEWS.PAGES.DETAILS.FORM.NAME.LABEL')"
            v-model="newsArticle.name"
            required :max-length="1024"
            :error="errors.length > 0" :error-message="errorMessage" @update:model-value="handleChange" />
        </Field>

        <div class="tw-flex tw-flex-row tw-gap-4">
          <Field
            :label="$t('VC_NEWS.PAGES.DETAILS.FORM.STORE.LABEL')"
            :model-value="newsArticle.storeId"
            name="storeId"
            rules="required"
            v-slot="{ errors, errorMessage, handleChange }">
            <VcSelect
              :label="$t('VC_NEWS.PAGES.DETAILS.FORM.STORE.LABEL')"
              v-model="newsArticle.storeId"
              :options="storeOptions"
              required
              :error="errors.length > 0" :error-message="errorMessage" @update:model-value="handleChange"
              class="tw-flex-auto" />
          </Field>

          <VcInput
            type="datetime-local"
            :label="$t('VC_NEWS.PAGES.DETAILS.FORM.PUBLISH_DATE.LABEL')"
            v-model="newsArticle.publishDate" />
        </div>

        <VcMultivalue
          :label="$t('VC_NEWS.PAGES.DETAILS.FORM.USER_GROUPS.LABEL')"
          v-model="userGroupsSelected"
          :options="userGroupsOptions" option-value="id" option-label="title"
          multivalue />

        <Field
          :label="$t('VC_NEWS.PAGES.DETAILS.FORM.CONTENT_TITLE.LABEL')"
          :model-value="selectedLocalizedContent.title"
          name="content-title"
          :rules="{ required: !!selectedLocalizedContent.content || !!selectedLocalizedContent.contentPreview }"
          v-slot="{ errors, errorMessage, handleChange }">
          <VcInput v-if="props.param"
            :label="$t('VC_NEWS.PAGES.DETAILS.FORM.CONTENT_TITLE.LABEL')"
            v-model="selectedLocalizedContent.title"
            :required="!!selectedLocalizedContent.content || !!selectedLocalizedContent.contentPreview"
            :error="errors.length > 0" :error-message="errorMessage" @update:model-value="handleChange"
            multilanguage :current-language="currentLocale">
          </VcInput>
        </Field>

        <VcEditor v-if="props.param"
          :label="$t('VC_NEWS.PAGES.DETAILS.FORM.CONTENT_PREVIEW.LABEL')"
          v-model="selectedLocalizedContent.contentPreview"
          multilanguage :current-language="currentLocale"
          assets-folder="news-articles" />

        <Field
          :label="$t('VC_NEWS.PAGES.DETAILS.FORM.CONTENT_CONTENT.LABEL')"
          :model-value="selectedLocalizedContent.content"
          name="content-content"
          :rules="{ required: !!selectedLocalizedContent.title || !!selectedLocalizedContent.contentPreview }"
          v-slot="{ errors, errorMessage, handleChange }">
          <VcEditor v-if="props.param"
            :label="$t('VC_NEWS.PAGES.DETAILS.FORM.CONTENT_CONTENT.LABEL')"
            v-model="selectedLocalizedContent.content"
            :required="!!selectedLocalizedContent.title || !!selectedLocalizedContent.contentPreview"
            :error="errors.length > 0" :error-message="errorMessage" @update:model-value="handleChange"
            multilanguage :current-language="currentLocale"
            assets-folder="news-articles" />
        </Field>

        <VcCard v-if="props.param"
          :header="$t('VC_NEWS.PAGES.DETAILS.FORM.SEO_HEADER.LABEL')"
          is-collapsable is-collapsed
          class="tw-flex tw-flex-col tw-gap-4 tw-p-4">
          <VcSwitch
            :label="$t('VC_NEWS.PAGES.DETAILS.FORM.SEO_IS_ACTIVE.LABEL')"
            v-model="selectedSeo.isActive" />

          <Field
            :label="$t('VC_NEWS.PAGES.DETAILS.FORM.SEO_SEMANTIC_URL.LABEL')"
            :model-value="selectedSeo.semanticUrl"
            name="seo-url"
            :rules="{ required: selectedSeo.isActive === true || !!selectedSeo.pageTitle || !!selectedSeo.metaDescription || !!selectedSeo.metaKeywords || !!selectedSeo.imageAltDescription }"
            v-slot="{ errors, errorMessage, handleChange }">
            <VcInput
              :label="$t('VC_NEWS.PAGES.DETAILS.FORM.SEO_SEMANTIC_URL.LABEL')"
              v-model="selectedSeo.semanticUrl"
              :required="selectedSeo.isActive === true || !!selectedSeo.pageTitle || !!selectedSeo.metaDescription || !!selectedSeo.metaKeywords || !!selectedSeo.imageAltDescription"
              :error="errors.length > 0" :error-message="errorMessage" @update:model-value="handleChange"
              multilanguage :current-language="currentLocale" />
          </Field>

          <VcInput
            :label="$t('VC_NEWS.PAGES.DETAILS.FORM.SEO_PAGE_TITLE.LABEL')"
            v-model="selectedSeo.pageTitle"
            multilanguage :current-language="currentLocale" />

          <VcTextarea
            :label="$t('VC_NEWS.PAGES.DETAILS.FORM.SEO_META_DESCRIPTION.LABEL')"
            v-model="selectedSeo.metaDescription"
            multilanguage :current-language="currentLocale" />

          <VcInput
            :label="$t('VC_NEWS.PAGES.DETAILS.FORM.SEO_META_KEYWORDS.LABEL')"
            v-model="selectedSeo.metaKeywords"
            multilanguage :current-language="currentLocale" />

          <VcInput
            :label="$t('VC_NEWS.PAGES.DETAILS.FORM.SEO_IMAGE_ALT_TEXT.LABEL')"
            v-model="selectedSeo.imageAltDescription"
            multilanguage :current-language="currentLocale" />
        </VcCard>
      </VcForm>
    </VcContainer>
  </VcBlade>
</template>

<script lang="ts" setup>
import { onMounted, ref, Ref, computed } from "vue";
import { useI18n } from "vue-i18n";
import { Field, useForm } from "vee-validate";
import { IBladeToolbar, IParentCallArgs, VcLanguageSelector, usePermissions, useBladeNavigation, usePopup } from "@vc-shell/framework";
import { useNewsArticleDetails, useNewsArticlePermissions, useStore, useUserGroups, useLocalization } from "../composables";
import { NewsArticleLocalizedContent, SeoInfo } from "../../../api_client/virtocommerce.news";

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
  param: undefined
});

defineOptions({
  url: "/vc-news-details",
  name: "NewsArticleDetails",
});

const { t } = useI18n({ useScope: "global" });

const { onBeforeClose } = useBladeNavigation();
const { hasAccess } = usePermissions();
const { showConfirmation } = usePopup();

const { meta } = useForm({ validateOnMount: true });

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
  }
});

//localization
const { languages, loadLanguages, loadingLanguages } = useLocalization();

const currentLocale = ref<string>("en-US");
const setLocale = (locale: string) => {
  currentLocale.value = locale;
};

const selectedLocalizedContent = computed(
  () => {
    if (newsArticle.value) {
      if (!newsArticle.value.localizedContents) {
        newsArticle.value.localizedContents = [];
      }
    }
    const existingLocalizedContent = newsArticle.value?.localizedContents?.find((x) => x.languageCode === currentLocale.value);

    if (existingLocalizedContent) {
      return existingLocalizedContent;
    }

    const newLocalizedContent = new NewsArticleLocalizedContent();
    newLocalizedContent.languageCode = currentLocale.value;
    newsArticle.value?.localizedContents?.push(newLocalizedContent);
    return newLocalizedContent;
  }
);

const selectedSeo = computed(
  () => {
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
  }
);

//news article 
const { newsArticle, loadNewsArticle, saveNewsArticle, loadingOrSavingNewsArticle, publishNewsArticle, unpublishNewsArticle, newsArticleCanPublish, newsArticleCanUnpublish, newsArticleIsDirty, resetNewsArticle } = useNewsArticleDetails();
const { publishNewsArticlePermission, createNewsArticlePermission, updateNewsArticlePermission } = useNewsArticlePermissions();

const saveNewsArticlePermission = props.param ? updateNewsArticlePermission : createNewsArticlePermission;

//other
const loading = computed(() => loadingStores.value || loadingUserGroups.value || loadingLanguages.value || loadingOrSavingNewsArticle.value);

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
    id: "publish",
    icon: "material-visibility",
    title: computed(() => t("VC_NEWS.PAGES.DETAILS.TOOLBAR.PUBLISH")),
    disabled: computed(() => !newsArticleCanPublish?.value),
    clickHandler: async () => {
      await publishNewsArticle();
      emit("parent:call", { method: "reload" });
      emit("parent:call", { method: "reOpenDetailsBlade", args: newsArticle.value!.id });
    },
    isVisible: computed(() => hasAccess(publishNewsArticlePermission)),
  });
  bladeToolbar.value.push({
    id: "unpublish",
    icon: "material-visibility-off",
    title: computed(() => t("VC_NEWS.PAGES.DETAILS.TOOLBAR.UNPUBLISH")),
    disabled: computed(() => !newsArticleCanUnpublish?.value),
    clickHandler: async () => {
      await unpublishNewsArticle();
      emit("parent:call", { method: "reload" });
      emit("parent:call", { method: "reOpenDetailsBlade", args: newsArticle.value!.id });
    },
    isVisible: computed(() => hasAccess(publishNewsArticlePermission)),
  });
}

const title = computed(() => t("VC_NEWS.PAGES.DETAILS.TITLE"));

onMounted(async () => {
  await loadStores();
  await loadUserGroups();
  await loadLanguages();
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