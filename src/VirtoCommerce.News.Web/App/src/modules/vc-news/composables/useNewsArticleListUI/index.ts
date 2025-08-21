import { ref, Ref, computed } from "vue";
import { ITableColumns, IBladeToolbar, useBladeNavigation, usePopup, usePermissions } from "@vc-shell/framework";
import { useI18n } from "vue-i18n";
import useNewsArticlePermissions from "../useNewsArticlePermissions";

export default (options: {
  selectedItemId: Ref<string | undefined>;
  selectedIds: Ref<string[]>;
  searchNewsArticles: () => Promise<void>;
  deleteNewsArticles: (args: { ids: string[] }) => Promise<void>;
}) => {
  const { t } = useI18n({ useScope: "global" });
  const { showConfirmation } = usePopup();
  const { openBlade, closeBlade } = useBladeNavigation();
  const { hasAccess } = usePermissions();
  const { createNewsArticlePermission, deleteNewsArticlePermission } = useNewsArticlePermissions();

  const bladeToolbar = computed((): IBladeToolbar[] => [
    {
      id: "refresh",
      title: t("VC_NEWS.PAGES.LIST.TOOLBAR.REFRESH"),
      icon: "material-refresh",
      async clickHandler() {
        await options.searchNewsArticles();
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
      disabled: options.selectedIds.value.length === 0,
      clickHandler: async () => {
        const confirmed = await showConfirmation(
          t("VC_NEWS.PAGES.LIST.ALERTS.DELETE_SELECTED_CONFIRMATION_MESSAGE", {
            count: options.selectedIds.value.length,
          }),
        );
        if (confirmed) {
          closeBlade(1);
          await options.deleteNewsArticles({ ids: options.selectedIds.value });
          options.selectedIds.value = [];
          await options.searchNewsArticles();
        }
      },
      isVisible: () => hasAccess(deleteNewsArticlePermission),
    },
  ]);

  const columns = ref<ITableColumns[]>([
    {
      id: "name",
      title: computed(() => t("VC_NEWS.PAGES.LIST.TABLE.HEADER.NAME")),
      alwaysVisible: true,
      sortable: true,
      width: "40%",
      mobilePosition: "top-left",
    },
    {
      id: "storeId",
      title: computed(() => t("VC_NEWS.PAGES.LIST.TABLE.HEADER.STORE")),
      alwaysVisible: true,
      sortable: true,
      width: "20%",
      mobilePosition: "top-right",
    },
    {
      id: "isPublished",
      title: computed(() => t("VC_NEWS.PAGES.LIST.TABLE.HEADER.IS_PUBLISHED")),
      alwaysVisible: true,
      sortable: true,
      width: "10%",
      type: "status-icon",
      mobilePosition: "status",
    },
    {
      id: "publishDate",
      title: computed(() => t("VC_NEWS.PAGES.LIST.TABLE.HEADER.PUBLISH_DATE")),
      alwaysVisible: true,
      sortable: true,
      width: "30%",
      type: "date-time",
      mobilePosition: "bottom-left",
    },
    {
      id: "isArchived",
      title: computed(() => t("VC_NEWS.PAGES.LIST.TABLE.HEADER.IS_ARCHIVED")),
      alwaysVisible: false,
      sortable: true,
      width: "10%",
    },
    {
      id: "archiveDate",
      title: computed(() => t("VC_NEWS.PAGES.LIST.TABLE.HEADER.ARCHIVE_DATE")),
      alwaysVisible: false,
      sortable: true,
      width: "30%",
      type: "date-time",
    },
    {
      id: "createdDate",
      title: computed(() => t("VC_NEWS.PAGES.LIST.TABLE.HEADER.CREATED_DATE")),
      visible: false,
      sortable: true,
      width: "30%",
      type: "date-time",
    },
    {
      id: "createdBy",
      title: computed(() => t("VC_NEWS.PAGES.LIST.TABLE.HEADER.CREATED_BY")),
      visible: false,
      sortable: true,
      width: "20%",
    },
    {
      id: "modifiedDate",
      title: computed(() => t("VC_NEWS.PAGES.LIST.TABLE.HEADER.MODIFIED_DATE")),
      visible: false,
      sortable: true,
      width: "30%",
      type: "date-time",
    },
    {
      id: "modifiedBy",
      title: computed(() => t("VC_NEWS.PAGES.LIST.TABLE.HEADER.MODIFIED_BY")),
      visible: false,
      sortable: true,
      width: "20%",
    },
    {
      id: "id",
      title: computed(() => t("VC_NEWS.PAGES.LIST.TABLE.HEADER.ID")),
      visible: false,
      width: "30%",
    },
  ]);

  const openDetailsBlade = (id: string | undefined) => {
    openBlade({
      blade: { name: "NewsArticleDetails" },
      param: id ?? undefined,
      onOpen() {
        options.selectedItemId.value = id ?? undefined;
      },
      onClose() {
        options.selectedItemId.value = undefined;
      },
    });
  };

  const reOpenDetailsBlade = (id: string) => {
    closeBlade(1);
    openDetailsBlade(id);
  };

  return {
    bladeToolbar,
    columns,
    openDetailsBlade,
    reOpenDetailsBlade,
  };
};
