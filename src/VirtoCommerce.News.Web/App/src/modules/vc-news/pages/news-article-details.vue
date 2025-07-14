<template>
  <VcBlade v-loading="loading" :title="$t('VC_NEWS.PAGES.DETAILS.TITLE')" :expanded="expanded" :closable="closable"
    width="70%" :toolbar-items="bladeToolbar" @close="$emit('close:blade')" @expand="$emit('expand:blade')"
    @collapse="$emit('collapse:blade')">
    <VcContainer>
      <VcForm class="tw-flex tw-flex-col tw-gap-4">
        <Field name="name" :model-value="item.name" :label="$t('VC_NEWS.PAGES.DETAILS.FORM.NAME.LABEL')"
          rules="required">
          <VcInput v-model="item.name" :label="$t('VC_NEWS.PAGES.DETAILS.FORM.NAME.LABEL')" :max-length="1024"
            required />
        </Field>

        <Field name="storeId" :model-value="item.storeId" :label="$t('VC_NEWS.PAGES.DETAILS.FORM.STORE.LABEL')"
          rules="required">
          <VcSelect v-model="item.storeId" :label="$t('VC_NEWS.PAGES.DETAILS.FORM.STORE.LABEL')" :options="storeOptions"
            required />
        </Field>

        <VcInput type="datetime-local" v-model="item.publishDate"
          :label="$t('VC_NEWS.PAGES.DETAILS.FORM.PUBLISH_DATE.LABEL')" />

        <VcMultivalue v-model="userGroupsSelected" :label="$t('VC_NEWS.PAGES.DETAILS.FORM.USER_GROUPS.LABEL')"
          :options="userGroupsOptions" option-value="id" option-label="title" multivalue />

        <div
          class="tw-flex tw-flex-row tw-justify-end">
          <VcLanguageSelector
            :model-value="currentLocale"
            :options="languages"
            @update:model-value="setLocale" />
        </div>

        <VcInput v-model="selectedLocalizedContent.title"
          :label="$t('VC_NEWS.PAGES.DETAILS.FORM.CONTENT_TITLE.LABEL')" />

        <VcEditor
          v-model="selectedLocalizedContent.content"
          :label="$t('VC_NEWS.PAGES.DETAILS.FORM.CONTENT_CONTENT.LABEL')"
          :placeholder="$t('VC_NEWS.PAGES.DETAILS.FORM.CONTENT_CONTENT.PLACEHOLDER')"
          assets-folder="news-articles" />

        <VcInput v-model="selectedLocalizedContent.contentPreview"
          :label="$t('VC_NEWS.PAGES.DETAILS.FORM.CONTENT_PREVIEW.LABEL')" />
      </VcForm>
    </VcContainer>
  </VcBlade>
</template>

<script lang="ts" setup>
import { IBladeToolbar, IParentCallArgs, VcLanguageSelector } from "@vc-shell/framework";
import { useNewsArticleDetails } from "../composables";
import { onMounted, ref, computed } from "vue";
import { Field } from "vee-validate";
import { useI18n } from "vue-i18n";

export interface Props {
  expanded?: boolean;
  closable?: boolean;
  param?: string;
}

export interface Emits {
  (event: "parent:call", args: IParentCallArgs): void;
  (event: "collapse:blade"): void;
  (event: "expand:blade"): void;
  (event: "close:blade"): void;
}

defineOptions({
  url: "/vc-news-details",
  name: "NewsArticleDetails",
});

const props = withDefaults(defineProps<Props>(), {
  expanded: true,
  closable: true,
  param: undefined,
});

const emit = defineEmits<Emits>();

const { t } = useI18n({ useScope: "global" });


const userGroupsOptions = computed(() => userGroups.value.map((x) => ({ id: x, title: x })));

const storeOptions = computed(() => stores.value.map((x) => ({ id: x.id, title: x.name })));

const userGroupsSelected = computed({
  get() {
    return item.value?.userGroups?.map((x) => ({ id: x, title: x })) ?? [];
  },
  set(newValue) {
    item.value!.userGroups = newValue.map((x) => x.id);
  }
});

const { item, stores, userGroups, loading, get, getStores, getUserGroups, save, currentLocale, setLocale, languages, getLanguages, selectedLocalizedContent } = useNewsArticleDetails();

const bladeToolbar = ref<IBladeToolbar[]>([
  {
    id: "save",
    icon: "material-save",
    title: t("VC_NEWS.PAGES.DETAILS.TOOLBAR.SAVE"),

    clickHandler: async () => {
      try {
        if (item.value) {
          await save(item.value);
          emit("parent:call", { method: "reload" });
        }
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
  await getStores();
  await getUserGroups();
  await getLanguages();
  if (props.param) {
    await get({ id: props.param });
  }
});
</script>
