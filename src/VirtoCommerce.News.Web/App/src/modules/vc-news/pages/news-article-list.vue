<template>
  <VcBlade
    :title="$t('VC_NEWS.PAGES.LIST.TITLE')"
    :toolbar-items="bladeToolbar"
    :closable="closable" :expanded="expanded"
    v-loading="loadingNewsArticles"
    width="40%"
    @close="$emit('close:blade')" @expand="$emit('expand:blade')" @collapse="$emit('collapse:blade')">

    <!-- @vue-generic {never} -->
    <VcTable
      :total-label="$t('VC_NEWS.PAGES.LIST.TABLE.TOTALS')" :search-placeholder="$t('VC_NEWS.PAGES.LIST.SEARCH.PLACEHOLDER')"
      :items="newsArticles" :selected-item-id="selectedItemId"
      :search-value="searchKeyword"
      :columns="columns"
      :sort="searchQuery.sort" :pages="pagesCount" :current-page="pageIndex" :total-count="newsArticlesCount"
      :expanded="expanded"
      @item-click="onItemClick" @header-click="onHeaderClick" @pagination-click="onPaginationClick"
      @search:change="onSearchChange" @selection-changed="onSelectionChanged"
      state-key="VC_NEWS"
      multiselect
      class="tw-grow tw-basis-0">
    </VcTable>
  </VcBlade>
</template>

<script lang="ts" setup>
import { computed, ref, markRaw, onMounted, watch } from "vue";
import { IBladeEvent, IBladeToolbar, IParentCallArgs, ITableColumns, usePermissions, useBladeNavigation, usePopup } from "@vc-shell/framework";
import { useI18n } from "vue-i18n";
import { useNewsArticleList, useNewsArticlePermissions } from "../composables";
import NewsArticleDetails from "./news-article-details.vue";
import { NewsArticle } from "src/api_client/virtocommerce.news";

export interface Props {
  expanded?: boolean;
  closable?: boolean;
  param?: string;
}

export interface Emits {
  (event: "parent:call", args: IParentCallArgs): void;
  (event: "collapse:blade"): void;
  (event: "expand:blade"): void;
  (event: "open:blade", blade: IBladeEvent): void;
  (event: "close:blade"): void;
}

defineOptions({
  url: "/vc-news",
  name: "NewsArticleList",
  isWorkspace: true,
  menuItem: {
    title: "VC_NEWS.MENU.TITLE",
    icon: "lucide-file",
    priority: 1,
  },
});

const props = withDefaults(defineProps<Props>(), {
  expanded: true,
  closable: true,
});

defineEmits<Emits>();

const { showConfirmation } = usePopup();

const { t } = useI18n({ useScope: "global" });
const { openBlade, closeBlade } = useBladeNavigation();
const { hasAccess } = usePermissions();
const { newsArticles, newsArticlesCount, pagesCount, pageIndex, searchNewsArticles, searchQuery, loadingNewsArticles, deleteNewsArticles } = useNewsArticleList();
const { createNewsArticlePermission, deleteNewsArticlePermission } = useNewsArticlePermissions();

const searchKeyword = ref();
const selectedItemId = ref<string>();
const selectedIds = ref<string[]>([]);

watch(
  () => props.param,
  (newVal) => {
    selectedItemId.value = newVal;
  },
  { immediate: true },
);

const bladeToolbar = computed((): IBladeToolbar[] => [
  {
    id: "refresh",
    title: computed(() => t("VC_NEWS.PAGES.LIST.TOOLBAR.REFRESH")),
    icon: "material-refresh",
    async clickHandler() {
      await reload();
    },
  },
  {
    id: "add",
    title: computed(() => t("VC_NEWS.PAGES.LIST.TOOLBAR.ADD")),
    icon: "material-add",
    clickHandler: async () => {
      openDetailsBlade(undefined);
    },
    isVisible: computed(() => hasAccess(createNewsArticlePermission)),
  },
  {
    id: "delete",
    title: computed(() => t("VC_NEWS.PAGES.LIST.TOOLBAR.DELETE")),
    icon: "material-delete",
    disabled: selectedIds.value.length === 0,
    clickHandler: async () => {
      const confirmed = await showConfirmation(t("VC_NEWS.PAGES.LIST.ALERTS.DELETE_SELECTED_CONFIRMATION_MESSAGE", { count: selectedIds.value.length }));
      if (confirmed) {
        closeBlade(1);
        await deleteNewsArticles({ ids: selectedIds.value });
        selectedIds.value = [];
        await reload();
      }
    },
    isVisible: computed(() => hasAccess(deleteNewsArticlePermission)),
  }
]);

const columns = ref<ITableColumns[]>([
  {
    id: "name",
    title: computed(() => t("VC_NEWS.PAGES.LIST.TABLE.HEADER.NAME")),
    alwaysVisible: true,
    sortable: true,
    width: "50%"
  },
  {
    id: "storeId",
    title: computed(() => t("VC_NEWS.PAGES.LIST.TABLE.HEADER.STORE")),
    alwaysVisible: true,
    sortable: true,
    width: "30%"
  },
  {
    id: "isPublished",
    title: computed(() => t("VC_NEWS.PAGES.LIST.TABLE.HEADER.IS_PUBLISHED")),
    alwaysVisible: true,
    sortable: true,
    width: "20%"
  },
]);

const title = computed(() => t("VC_NEWS.PAGES.LIST.TITLE"));

const reload = async () => {
  await searchNewsArticles();
};

const onItemClick = (item: { id: string }) => {
  openDetailsBlade(item.id);
};

const openDetailsBlade = (id: string | undefined) => {
  openBlade({
    blade: markRaw(NewsArticleDetails),
    param: id ?? undefined,
    onOpen() {
      selectedItemId.value = id ?? undefined;
    },
    onClose() {
      selectedItemId.value = undefined;
    },
  });
}

const reOpenDetailsBlade = (id: string) => {
  closeBlade(1);
  openDetailsBlade(id);
}

const onSearchChange = (searchKeywordValue: string | undefined) => {
  searchQuery.value.searchPhrase = searchKeywordValue;
  searchNewsArticles();
}

const onPaginationClick = (page: number) => {
  pageIndex.value = page;
  searchNewsArticles();
}

const onHeaderClick = async (item: ITableColumns) => {
  const sortOptions = ["DESC", "ASC", ""];

  if (item.sortable) {
    if (searchQuery?.value?.sort?.split(":")[0] === item.id) {
      const index = sortOptions.findIndex((x) => {
        const sorting = searchQuery?.value?.sort?.split(":")[1];
        if (sorting) {
          return x === sorting;
        } else {
          return x === "";
        }
      });

      if (index !== -1) {
        const newSort = sortOptions[(index + 1) % sortOptions.length];

        if (newSort === "") {
          searchQuery.value.sort = '';
        } else {
          searchQuery.value.sort = `${item.id}:${newSort}`;
        }
        await searchNewsArticles();
      }
    } else {
      searchQuery.value.sort = `${item.id}:${sortOptions[0]}`;
      await searchNewsArticles();
    }
  }
};

const onSelectionChanged = function (selectedItems: NewsArticle[]) {
  selectedIds.value = selectedItems.map((item) => item.id || "").filter(Boolean);
}

onMounted(async () => {
  await searchNewsArticles();
});

defineExpose({
  title,
  reload,
  reOpenDetailsBlade
});
</script>
