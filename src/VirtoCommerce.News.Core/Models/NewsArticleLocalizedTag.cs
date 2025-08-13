using System;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.News.Core.Models;

public class NewsArticleLocalizedTag : Entity, ICloneable
{
    public string NewsArticleId { get; set; }
    public string LanguageCode { get; set; }
    public string Tag { get; set; }

    public object Clone() => MemberwiseClone();
}
