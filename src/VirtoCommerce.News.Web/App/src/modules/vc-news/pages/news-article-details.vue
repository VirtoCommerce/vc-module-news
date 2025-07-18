<template>
  <VcBlade
    :title="$t('VC_NEWS.PAGES.DETAILS.TITLE')"
    :toolbar-items="bladeToolbar"
    :closable="closable" :expanded="expanded"
    v-loading="loading"
    width="60%"
    @close="$emit('close:blade')" @expand="$emit('expand:blade')" @collapse="$emit('collapse:blade')">
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

        <VcInput v-if="props.param"
          :label="$t('VC_NEWS.PAGES.DETAILS.FORM.CONTENT_TITLE.LABEL')"
          v-model="selectedLocalizedContent.title"
          required
          multilanguage :current-language="currentLocale">
          <template #prepend>
            <VcLanguageSelector
              :model-value="currentLocale"
              :options="languages"
              @update:model-value="setLocale" />
          </template>
        </VcInput>

        <VcEditor v-if="props.param"
          :label="$t('VC_NEWS.PAGES.DETAILS.FORM.CONTENT_CONTENT.LABEL')"
          v-model="selectedLocalizedContent.content"
          required
          multilanguage :current-language="currentLocale"
          assets-folder="news-articles" />

        <VcEditor v-if="props.param"
          :label="$t('VC_NEWS.PAGES.DETAILS.FORM.CONTENT_PREVIEW.LABEL')"
          v-model="selectedLocalizedContent.contentPreview"
          multilanguage :current-language="currentLocale"
          assets-folder="news-articles" />

        <VcCard v-if="props.param"
          :header="$t('VC_NEWS.PAGES.DETAILS.FORM.SEO_HEADER.LABEL')"
          is-collapsable is-collapsed
          class="tw-flex tw-flex-col tw-gap-4 tw-p-4">
          <VcSwitch
            :label="$t('VC_NEWS.PAGES.DETAILS.FORM.SEO_IS_ACTIVE.LABEL')"
            v-model="selectedSeo.isActive" />

          <VcInput
            :label="$t('VC_NEWS.PAGES.DETAILS.FORM.SEO_SEMANTIC_URL.LABEL')"
            v-model="selectedSeo.semanticUrl"
            multilanguage :current-language="currentLocale" />

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
import { onMounted, ref, computed } from "vue";
import { Field } from "vee-validate";
import { useI18n } from "vue-i18n";
import { IBladeToolbar, IParentCallArgs, VcLanguageSelector } from "@vc-shell/framework";
import { useNewsArticleDetails, useStore, useUserGroups, useLocalization } from "../composables";
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


//other
const loading = computed(() => loadingStores.value || loadingUserGroups.value || loadingLanguages.value || loadingOrSavingNewsArticle.value);

const bladeToolbar = ref<IBladeToolbar[]>([
  {
    id: "save",
    icon: "material-save",
    title: t("VC_NEWS.PAGES.DETAILS.TOOLBAR.SAVE"),
    disabled: computed(() => !newsArticleIsDirty?.value),
    clickHandler: async () => {
      try {
        await saveNewsArticle();
        emit('close:blade');
        emit("parent:call", { method: "reload" });
        emit("parent:call", {
          method: "openDetailsBlade", args: newsArticle.value!.id
        });
      } catch (error) {
        console.error("Failed to save news article:", error);
      }
    },
  },
  {
    id: "reset",
    icon: "material-undo",
    title: t("VC_NEWS.PAGES.DETAILS.TOOLBAR.RESET"),
    disabled: computed(() => !newsArticleIsDirty?.value),
    clickHandler: async () => {
      resetNewsArticle();
    },
  },
  {
    id: "publish",
    icon: "material-visibility",
    title: t("VC_NEWS.PAGES.DETAILS.TOOLBAR.PUBLISH"),
    disabled: computed(() => !newsArticleCanPublish?.value),
    clickHandler: async () => {
      await publishNewsArticle();
      emit("parent:call", { method: "reload" });
      emit("parent:call", {
        method: "openDetailsBlade", args: newsArticle.value!.id
      });
    },
  },
  {
    id: "unpublish",
    icon: "material-visibility-off",
    title: t("VC_NEWS.PAGES.DETAILS.TOOLBAR.UNPUBLISH"),
    disabled: computed(() => !newsArticleCanUnpublish?.value),
    clickHandler: async () => {
      await unpublishNewsArticle();
      emit("parent:call", { method: "reload" });
      emit("parent:call", {
        method: "openDetailsBlade", args: newsArticle.value!.id
      });
    },
  }
]);

onMounted(async () => {
  await loadStores();
  await loadUserGroups();
  await loadLanguages();
  if (props.param) {
    await loadNewsArticle({ id: props.param });
  }
});
</script>
