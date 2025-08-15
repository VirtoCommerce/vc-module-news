<template>
  <VcBlade
    v-loading="loadingNewsArticles"
    :title="title"
    :toolbar-items="bladeToolbar"
    :closable="closable"
    :expanded="expanded"
    width="40%"
    @close="$emit('close:blade')"
    @expand="$emit('expand:blade')"
    @collapse="$emit('collapse:blade')"
  >
    <!-- @vue-generic {NewsArticle} -->
    <VcTable
      :total-label="$t('VC_NEWS.PAGES.LIST.TABLE.TOTALS')"
      :search-placeholder="$t('VC_NEWS.PAGES.LIST.SEARCH.PLACEHOLDER')"
      :items="newsArticles"
      :selected-item-id="selectedItemId"
      :search-value="searchKeyword"
      :columns="columns"
      :sort="sortExpression"
      :pages="pagesCount"
      :current-page="pageIndex"
      :total-count="newsArticlesCount"
      :expanded="expanded"
      column-selector="defined"
      state-key="VC_NEWS"
      multiselect
      class="tw-grow tw-basis-0"
      @item-click="onItemClick"
      @header-click="onHeaderClick"
      @pagination-click="onPaginationClick"
      @search:change="onSearchChange"
      @selection-changed="onSelectionChanged"
    >
    </VcTable>
  </VcBlade>
</template>

<script lang="ts" setup>
import { computed, ref, markRaw, onMounted, watch } from "vue";
import {
  IBladeToolbar,
  IParentCallArgs,
  ITableColumns,
  usePermissions,
  useBladeNavigation,
  usePopup,
  useTableSort,
} from "@vc-shell/framework";
import { useI18n } from "vue-i18n";
import { useNewsArticleListUI, useNewsArticleList, useNewsArticlePermissions } from "../composables";
import NewsArticleDetails from "./news-article-details.vue";
import { NewsArticle } from "../../../api_client/virtocommerce.news";

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
  url: "/list-all",
  name: "NewsArticleListAll",
  isWorkspace: true,
  menuItem: {
    title: "VC_NEWS.MENU.ALL",
    icon: "lucide-file",
    priority: 50,
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
const {
  newsArticles,
  newsArticlesCount,
  pagesCount,
  pageIndex,
  searchQuery,
  searchNewsArticlesAll: searchNewsArticles,
  loadingNewsArticlesAll: loadingNewsArticles,
  deleteNewsArticles,
} = useNewsArticleList();
const { createNewsArticlePermission, deleteNewsArticlePermission } = useNewsArticlePermissions();

const { sortExpression, handleSortChange } = useTableSort({ initialProperty: "createdDate", initialDirection: "ASC" });

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
    title: t("VC_NEWS.PAGES.LIST.TOOLBAR.REFRESH"),
    icon: "material-refresh",
    async clickHandler() {
      await reload();
    },
  },
  {
    id: "add",
    title: t("VC_NEWS.PAGES.LIST.TOOLBAR.ADD"),
    icon: "material-add",
    clickHandler: async () => {
      openDetailsBlade(undefined);
    },
    isVisible: () => hasAccess(createNewsArticlePermission),
  },
  {
    id: "delete",
    title: t("VC_NEWS.PAGES.LIST.TOOLBAR.DELETE"),
    icon: "material-delete",
    disabled: selectedIds.value.length === 0,
    clickHandler: async () => {
      const confirmed = await showConfirmation(
        t("VC_NEWS.PAGES.LIST.ALERTS.DELETE_SELECTED_CONFIRMATION_MESSAGE", { count: selectedIds.value.length }),
      );
      if (confirmed) {
        closeBlade(1);
        await deleteNewsArticles({ ids: selectedIds.value });
        selectedIds.value = [];
        await reload();
      }
    },
    isVisible: () => hasAccess(deleteNewsArticlePermission),
  },
]);

const { columns } = useNewsArticleListUI();

const title = computed(() => t("VC_NEWS.PAGES.LIST.TITLE"));

const reload = async () => {
  await searchNewsArticles();
};

const onItemClick = (item: NewsArticle) => {
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
};

const reOpenDetailsBlade = (id: string) => {
  closeBlade(1);
  openDetailsBlade(id);
};

const onSearchChange = (searchKeywordValue: string | undefined) => {
  searchQuery.value.searchPhrase = searchKeywordValue;
  searchNewsArticles();
};

const onPaginationClick = (page: number) => {
  pageIndex.value = page;
  searchNewsArticles();
};

const onHeaderClick = async (column: ITableColumns) => {
  if (column.sortable) {
    handleSortChange(column.id);
  }
};

const onSelectionChanged = function (selectedItems: NewsArticle[]) {
  selectedIds.value = selectedItems.map((item) => item.id || "").filter(Boolean);
};

onMounted(async () => {
  await searchNewsArticles();
});

watch(sortExpression, async (newSortValue) => {
  searchQuery.value.sort = newSortValue;
  await searchNewsArticles();
});

defineExpose({
  title,
  reload,
  reOpenDetailsBlade,
});
</script>
