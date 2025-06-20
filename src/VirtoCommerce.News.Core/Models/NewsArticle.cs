using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Seo.Core.Models;

namespace VirtoCommerce.News.Core.Models;

public class NewsArticle : AuditableEntity, ICloneable, ISeoSupport
{
    public string StoreId { get; set; }

    public string Name { get; set; }

    public bool IsPublished { get; set; }

    [JsonIgnore]
    public bool? IsPublishedValue { get; private set; }

    public DateTime? PublishDate { get; set; }

    public IList<NewsArticleLocalizedContent> LocalizedContents { get; set; }

    public string SeoObjectType => nameof(NewsArticle);//Question #SEO1

    public IList<SeoInfo> SeoInfos { get; set; }

    public object Clone()
    {
        var result = (NewsArticle)MemberwiseClone();

        result.SeoInfos = SeoInfos?.Select(x => x.Clone()).OfType<SeoInfo>().ToList();
        result.LocalizedContents = LocalizedContents?.Select(x => x.CloneTyped()).ToList();

        return result;
    }

    public void SetIsPublished(bool isPublished)
    {
        IsPublishedValue = isPublished;
    }
}
