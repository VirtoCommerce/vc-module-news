using System.Collections.Generic;
using VirtoCommerce.SearchModule.Core.Model;

namespace VirtoCommerce.News.Core.Models;

public class NewsArticleIndexedSearchResult : NewsArticleSearchResult
{
    public virtual IList<Aggregation> Aggregations { get; set; }
}
