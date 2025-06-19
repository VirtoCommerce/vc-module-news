using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.News.Core.Models;

public class NewsArticleSearchCriteria : SearchCriteriaBase
{
    public bool? Published { get; set; }
}
