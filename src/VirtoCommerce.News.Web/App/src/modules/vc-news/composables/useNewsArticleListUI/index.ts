import { ref, computed } from "vue";
import { ITableColumns } from "@vc-shell/framework";
import { useI18n } from "vue-i18n";

export default () => {
  const { t } = useI18n({ useScope: "global" });

  const columns = ref<ITableColumns[]>([
    {
      id: "name",
      title: computed(() => t("VC_NEWS.PAGES.LIST.TABLE.HEADER.NAME")),
      alwaysVisible: true,
      sortable: true,
      width: "40%",
    },
    {
      id: "storeId",
      title: computed(() => t("VC_NEWS.PAGES.LIST.TABLE.HEADER.STORE")),
      alwaysVisible: true,
      sortable: true,
      width: "20%",
    },
    {
      id: "isPublished",
      title: computed(() => t("VC_NEWS.PAGES.LIST.TABLE.HEADER.IS_PUBLISHED")),
      alwaysVisible: true,
      sortable: true,
      width: "10%",
    },
    {
      id: "publishDate",
      title: computed(() => t("VC_NEWS.PAGES.LIST.TABLE.HEADER.PUBLISH_DATE")),
      alwaysVisible: true,
      sortable: true,
      width: "30%",
      type: "date-time",
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

  return {
    columns,
  };
};
