using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.News.Core.Models;

public class NewsArticle : AuditableEntity, ICloneable
{
    public string Name { get; set; }
    public bool IsPublished { get; set; }
    public DateTime? PublishDate { get; set; }
    public IList<NewsArticleLocalizedContent> LocalizedContents { get; set; }

    public object Clone()
    {
        var result = (NewsArticle)MemberwiseClone();

        result.LocalizedContents = LocalizedContents?.Select(x => x.CloneTyped()).ToList();

        return result;
    }
}
