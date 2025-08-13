using System.Collections.Generic;

namespace VirtoCommerce.News.Web.Model;

public class NewsArticleOptions
{
    public IList<string> Tags { get; set; }
    public IList<string> PublishScopes { get; set; }
}
