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
import { computed, ref, onMounted, watch } from "vue";
import { IParentCallArgs, ITableColumns, useTableSort } from "@vc-shell/framework";
import { useI18n } from "vue-i18n";
import { useNewsArticleListUI, useNewsArticleList } from "../composables";
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

const { t } = useI18n({ useScope: "global" });
const {
  newsArticles,
  newsArticlesCount,
  pagesCount,
  pageIndex,
  searchQuery,
  searchNewsArticlesAll,
  loadingNewsArticles,
  deleteNewsArticles,
} = useNewsArticleList();

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

const title = computed(() => `${t("VC_NEWS.PAGES.LIST.TITLE")}: ${t("VC_NEWS.MENU.ALL")}`);

const { bladeToolbar, columns, openDetailsBlade, reOpenDetailsBlade } = useNewsArticleListUI({
  selectedItemId,
  selectedIds,
  searchNewsArticles: searchNewsArticlesAll,
  deleteNewsArticles,
});

const onItemClick = (item: NewsArticle) => {
  openDetailsBlade(item.id);
};

const onSearchChange = (searchKeywordValue: string | undefined) => {
  searchQuery.value.searchPhrase = searchKeywordValue;
  searchNewsArticlesAll();
};

const onPaginationClick = (page: number) => {
  pageIndex.value = page;
  searchNewsArticlesAll();
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
  await searchNewsArticlesAll();
});

watch(sortExpression, async (newSortValue) => {
  searchQuery.value.sort = newSortValue;
  await searchNewsArticlesAll();
});

defineExpose({
  title,
  reload: searchNewsArticlesAll,
  reOpenDetailsBlade,
});
</script>
