import { ref } from "vue";
import { useAsync, useApiClient } from "@vc-shell/framework";
import { CustomerModuleClient, Contact, MembersSearchCriteria } from "../../../../api_client/virtocommerce.customer";

export default () => {
  const { getApiClient: getCustomerApiClient } = useApiClient(CustomerModuleClient);

  const authors = ref<Contact[]>([]);

  const { loading: loadingAuthors, action: loadAuthors } = useAsync(async () => {
    const apiClient = await getCustomerApiClient();
    const apiResult = await apiClient.searchContacts(new MembersSearchCriteria({ take: 999, memberType: "Contact" }));

    if (apiResult && apiResult.results) {
      authors.value = apiResult.results;
    }
  });

  return {
    authors,
    loadAuthors,
    loadingAuthors,
  };
};
