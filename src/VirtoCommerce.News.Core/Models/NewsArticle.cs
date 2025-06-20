using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.News.Core.Models;

public class NewsArticle : AuditableEntity, ICloneable
{
    public string StoreId { get; set; }

    public string Name { get; set; }

    public bool IsPublished { get; set; }

    [JsonIgnore]
    public bool? IsPublishedValue { get; private set; }

    public DateTime? PublishDate { get; set; }

    public IList<NewsArticleLocalizedContent> LocalizedContents { get; set; }

    public object Clone()
    {
        var result = (NewsArticle)MemberwiseClone();

        result.LocalizedContents = LocalizedContents?.Select(x => x.CloneTyped()).ToList();

        return result;
    }

    public void SetIsPublished(bool isPublished)
    {
        IsPublishedValue = isPublished;
    }
}
