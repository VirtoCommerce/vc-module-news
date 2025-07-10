<template>
  <VcBlade v-loading="loading" :title="$t('VC_NEWS.PAGES.DETAILS.TITLE')" :expanded="expanded" :closable="closable"
    width="70%" :toolbar-items="bladeToolbar" @close="$emit('close:blade')" @expand="$emit('expand:blade')"
    @collapse="$emit('collapse:blade')">
    <VcContainer class="tw-p-2">
      <VcForm>
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
        <Field name="publishDate" :model-value="item.publishDate"
          :label="$t('VC_NEWS.PAGES.DETAILS.FORM.PUBLISH_DATE.LABEL')">
          <VcInput type="datetime-local" v-model="item.publishDate"
            :label="$t('VC_NEWS.PAGES.DETAILS.FORM.PUBLISH_DATE.LABEL')" />
        </Field>
        <Field name="storeId" :model-value="userGroups" :label="$t('VC_NEWS.PAGES.DETAILS.FORM.USER_GROUPS.LABEL')">
          <VcMultivalue v-model="userGroups" :label="$t('VC_NEWS.PAGES.DETAILS.FORM.USER_GROUPS.LABEL')"
            :options="userGroupsOptions" option-value="id" option-label="title" multivalue />
        </Field>
      </VcForm>
    </VcContainer>
  </VcBlade>
</template>

<script lang="ts" setup>
import { IBladeToolbar, IParentCallArgs } from "@vc-shell/framework";
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


const userGroupsOptions = [
  { id: 'VIP', title: 'VIP' },
];
const storeOptions = [
  { id: 'B2B-store', title: 'B2B-store' },
];

const userGroups = computed({
  get() {
    return item.value?.userGroups?.map((x) => ({ id: x, title: x })) ?? [];
  },
  set(newValue) {
    item.value!.userGroups = newValue.map((x) => x.id);
  }
});

const { item, loading, get, save } = useNewsArticleDetails();

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
  if (props.param) {
    await get({ id: props.param });
  }
});
</script>
