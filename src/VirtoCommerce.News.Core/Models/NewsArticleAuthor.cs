using System;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.News.Core.Models;

public class NewsArticleAuthor : AuditableEntity, ICloneable
{
    public string PhotoUrl { get; set; }

    public string Name { get; set; }

    public object Clone()
    {
        return MemberwiseClone();
    }
}
