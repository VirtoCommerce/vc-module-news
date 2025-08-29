using System;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.News.Core.Models;

public class NewsArticleLocalizedContent : AuditableEntity, ICloneable
{
    public string NewsArticleId { get; set; }
    public string LanguageCode { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public string ContentPreview { get; set; }
    public string ListTitle { get; set; }
    public string ListPreview { get; set; }

    public object Clone() => MemberwiseClone();
}
