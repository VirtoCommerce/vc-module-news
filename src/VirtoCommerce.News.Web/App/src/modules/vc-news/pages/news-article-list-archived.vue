<template>
  <VcBlade
    :loading="loadingNewsArticles"
    :title="title"
    :toolbar-items="bladeToolbar"
    width="40%"
  >
    <VcDataTable
      :items="newsArticles"
      :total-count="pagination.totalCount"
      :pagination="pagination"
      :searchable="true"
      :selection-mode="'multiple'"
      :search-placeholder="$t('VC_NEWS.PAGES.LIST.SEARCH.PLACEHOLDER')"
      state-key="VC_NEWS"
      class="tw-grow tw-basis-0"
      v-model:active-item-id="selectedItemId"
      v-model:sort-field="sortField"
      v-model:sort-order="sortOrder"
      v-model:selection="localSelection"
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
  url: "/list-archived",
  name: "NewsArticleListArchived",
  isWorkspace: true,
  menuItem: {
    title: "VC_NEWS.MENU.ARCHIVED",
    icon: "lucide-archive",
    priority: 40,
  },
});

const { t } = useI18n({ useScope: "global" });
const {
  newsArticles,
  newsArticlesCount,
  pageSize,
  pageIndex,
  searchQuery,
  searchNewsArticlesArchived,
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
    return searchNewsArticlesArchived();
  },
});

const selectedItemId = ref<string>();
const localSelection = ref<NewsArticle[]>([]);
const selectedIds = ref<string[]>([]);

watch(
  localSelection,
  (newSelection) => {
    selectedIds.value = newSelection.map((item) => item.id || "").filter(Boolean);
  },
  { deep: true },
);

watch(
  () => param.value,
  (newVal) => {
    selectedItemId.value = newVal;
  },
  { immediate: true },
);

const { bladeToolbar, columns, openDetailsBlade, reOpenDetailsBlade } = useNewsArticleListUI({
  selectedItemId,
  selectedIds,
  searchNewsArticles: searchNewsArticlesArchived,
  deleteNewsArticles,
});

const title = computed(() => `${t("VC_NEWS.PAGES.LIST.TITLE")}: ${t("VC_NEWS.MENU.ARCHIVED")}`);

const onItemClick = (event: { data: NewsArticle }) => {
  openDetailsBlade(event.data.id);
};

const onSearchChange = (searchKeywordValue: string | undefined) => {
  searchQuery.value.searchPhrase = searchKeywordValue;
  searchNewsArticlesArchived();
};

onMounted(async () => {
  await searchNewsArticlesArchived();
});

watch(sortExpression, async (newSortValue) => {
  searchQuery.value.sort = newSortValue;
  await searchNewsArticlesArchived();
});

exposeToChildren({
  reload: searchNewsArticlesArchived,
  reOpenDetailsBlade,
});
</script>
