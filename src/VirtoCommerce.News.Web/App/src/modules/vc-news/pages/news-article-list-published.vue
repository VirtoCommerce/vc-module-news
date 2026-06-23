<template>
  <VcBlade
    :loading="loadingNewsArticles"
    :title="title"
    :toolbar-items="bladeToolbar"
    width="40%"
  >
    <VcDataTable
      v-model:active-item-id="selectedItemId"
      v-model:sort-field="sortField"
      v-model:sort-order="sortOrder"
      v-model:selection="localSelection"
      :items="newsArticles"
      :total-count="pagination.totalCount"
      :pagination="pagination"
      :searchable="true"
      :selection-mode="'multiple'"
      :search-placeholder="$t('VC_NEWS.PAGES.LIST.SEARCH.PLACEHOLDER')"
      state-key="VC_NEWS"
      class="tw-grow tw-basis-0"
      @row-click="onItemClick"
      @pagination-click="pagination.goToPage"
      @search="onSearchChange"
    >
      <VcColumn
        v-for="col in columns"
        :id="col.id"
        :key="col.id"
        :title="col.title"
        :field="col.field"
        :width="col.width"
        :always-visible="col.alwaysVisible"
        :visible="col.visible"
        :sortable="col.sortable"
        :type="col.type"
        :mobile-position="col.mobilePosition"
        :mobile-role="col.mobileRole"
      />
    </VcDataTable>
  </VcBlade>
</template>

<script lang="ts" setup>
import { computed, ref, onMounted, watch } from "vue";
import { useDataTableSort, useDataTablePagination, useBlade } from "@vc-shell/framework";
import { useI18n } from "vue-i18n";
import { useNewsArticleListUI, useNewsArticleList } from "../composables";
import { NewsArticle } from "../../../api_client/virtocommerce.news";

import { VcBlade, VcDataTable, VcColumn } from "@vc-shell/framework/ui";

const { param, exposeToChildren } = useBlade();
defineBlade({
  url: "/list-published",
  name: "NewsArticleListPublished",
  isWorkspace: true,
  menuItem: {
    title: "VC_NEWS.MENU.PUBLISHED",
    icon: "lucide-eye",
    priority: 30,
  },
});

const { t } = useI18n({ useScope: "global" });
const {
  newsArticles,
  newsArticlesCount,
  pageSize,
  pageIndex,
  searchQuery,
  searchNewsArticlesPublished,
  loadingNewsArticles,
  deleteNewsArticles,
} = useNewsArticleList();

const { sortField, sortOrder, sortExpression } = useDataTableSort({
  initialField: "createdDate",
  initialDirection: "ASC",
});

const pagination = useDataTablePagination({
  pageSize,
  totalCount: computed(() => newsArticlesCount.value),
  onPageChange: ({ page }) => {
    pageIndex.value = page;
    return searchNewsArticlesPublished();
  },
});

const selectedItemId = ref<string>();
const localSelection = ref<NewsArticle[]>([]);

watch(
  () => param.value,
  (newVal) => {
    selectedItemId.value = newVal;
  },
  { immediate: true },
);

const { bladeToolbar, columns, openDetailsBlade, reOpenDetailsBlade } = useNewsArticleListUI({
  selectedItemId,
  selection: localSelection,
  searchNewsArticles: searchNewsArticlesPublished,
  deleteNewsArticles,
});

const title = computed(() => `${t("VC_NEWS.PAGES.LIST.TITLE")}: ${t("VC_NEWS.MENU.PUBLISHED")}`);

const onItemClick = (event: { data: NewsArticle }) => {
  openDetailsBlade(event.data.id);
};

const onSearchChange = (searchKeywordValue: string | undefined) => {
  searchQuery.value.searchPhrase = searchKeywordValue;
  searchNewsArticlesPublished();
};

onMounted(async () => {
  await searchNewsArticlesPublished();
});

watch(sortExpression, async (newSortValue) => {
  searchQuery.value.sort = newSortValue;
  await searchNewsArticlesPublished();
});

exposeToChildren({
  reload: searchNewsArticlesPublished,
  reOpenDetailsBlade,
});
</script>
