using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.News.Core.Models;

public class NewsArticleSearchCriteria : SearchCriteriaBase
{
    public bool? Published { get; set; }
    public string StoreId { get; set; }
    public IList<string> UserGroups { get; set; }
}
