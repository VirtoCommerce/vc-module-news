<template>
  <VcBlade v-loading="loading" :title="$t('VC_NEWS.PAGES.DETAILS.TITLE')" :expanded="expanded" :closable="closable"
    width="70%" :toolbar-items="bladeToolbar" @close="$emit('close:blade')" @expand="$emit('expand:blade')"
    @collapse="$emit('collapse:blade')">
    <VcContainer>
      <VcForm class="tw-flex tw-flex-col tw-gap-4">
        <Field name="name" :model-value="newsArticle.name" :label="$t('VC_NEWS.PAGES.DETAILS.FORM.NAME.LABEL')"
          rules="required">
          <VcInput v-model="newsArticle.name" :label="$t('VC_NEWS.PAGES.DETAILS.FORM.NAME.LABEL')" :max-length="1024"
            required />
        </Field>

        <div class="tw-flex tw-flex-row tw-gap-4">
          <Field name="storeId" :model-value="newsArticle.storeId" :label="$t('VC_NEWS.PAGES.DETAILS.FORM.STORE.LABEL')"
            rules="required">
            <VcSelect v-model="newsArticle.storeId" :label="$t('VC_NEWS.PAGES.DETAILS.FORM.STORE.LABEL')"
              :options="storeOptions"
              required
              class="tw-flex-auto" />
          </Field>

          <VcInput type="datetime-local" v-model="newsArticle.publishDate"
            :label="$t('VC_NEWS.PAGES.DETAILS.FORM.PUBLISH_DATE.LABEL')" />
        </div>

        <VcMultivalue v-model="userGroupsSelected" :label="$t('VC_NEWS.PAGES.DETAILS.FORM.USER_GROUPS.LABEL')"
          :options="userGroupsOptions" option-value="id" option-label="title" multivalue />

        <VcInput v-if="props.param"
          v-model="selectedLocalizedContent.title"
          :label="$t('VC_NEWS.PAGES.DETAILS.FORM.CONTENT_TITLE.LABEL')"
          required>
          <template #prepend>
            <VcLanguageSelector
              :model-value="currentLocale"
              :options="languages"
              @update:model-value="setLocale" />
          </template>
        </VcInput>

        <VcEditor v-if="props.param"
          v-model="selectedLocalizedContent.content"
          :label="$t('VC_NEWS.PAGES.DETAILS.FORM.CONTENT_CONTENT.LABEL')"
          assets-folder="news-articles" required />

        <VcInput v-if="props.param"
          v-model="selectedLocalizedContent.contentPreview"
          :label="$t('VC_NEWS.PAGES.DETAILS.FORM.CONTENT_PREVIEW.LABEL')" />
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
import { NewsArticleLocalizedContent } from "../../../api_client/virtocommerce.news";

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

//news article 
const { newsArticle, loadNewsArticle, saveNewsArticle, loadingOrSavingNewsArticle } = useNewsArticleDetails();


//other
const loading = computed(() => loadingStores.value || loadingUserGroups.value || loadingLanguages.value || loadingOrSavingNewsArticle.value);

const bladeToolbar = ref<IBladeToolbar[]>([
  {
    id: "save",
    icon: "material-save",
    title: t("VC_NEWS.PAGES.DETAILS.TOOLBAR.SAVE"),

    clickHandler: async () => {
      try {
        await saveNewsArticle();
        emit('close:blade');
        emit("parent:call", { method: "reload" });
        emit("parent:call", {
          method: "openDetailsBlade", args: newsArticle.value!.id
        });
      } catch (error) {
        console.error("Failed to save product:", error);
      }
    },
  },
  {
    id: "reset",
    icon: "material-undo",
    title: t("VC_NEWS.PAGES.DETAILS.TOOLBAR.RESET"),
    clickHandler: async () => {
      //reset here
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
