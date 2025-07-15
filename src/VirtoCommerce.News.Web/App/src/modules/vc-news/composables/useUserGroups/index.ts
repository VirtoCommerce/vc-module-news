import { ref } from "vue";
import { useAsync, useApiClient } from "@vc-shell/framework";
import { SettingClient } from "../../../../api_client/virtocommerce.platform";

export default () => {
  const { getApiClient: getSettingApiClient } = useApiClient(SettingClient);

  const userGroups = ref<string[]>([]);

  const { loading: loadingUserGroups, action: loadUserGroups } = useAsync(async () => {
    const apiClient = await getSettingApiClient();
    const apiResult = await apiClient.getGlobalSetting("Customer.MemberGroups");

    if (apiResult && apiResult.allowedValues) {
      userGroups.value = apiResult.allowedValues;
    }
  });

  return {
    userGroups,
    loadUserGroups,
    loadingUserGroups,
  };
};
