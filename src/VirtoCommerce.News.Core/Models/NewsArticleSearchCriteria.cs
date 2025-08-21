using System;
using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.News.Core.Models;

public class NewsArticleSearchCriteria : SearchCriteriaBase
{
    public string ContentKeyword { get; set; }
    public NewsArticleStatus? Status { get; set; }

    public string StoreId { get; set; }
    public IList<string> UserGroups { get; set; }
    public IList<string> LanguageCodes { get; set; }
    public DateTime? CertainDate { get; set; }
    public string PublishScope { get; set; }
    public string AuthorId { get; set; }
    public IList<string> Tags { get; set; }
}
