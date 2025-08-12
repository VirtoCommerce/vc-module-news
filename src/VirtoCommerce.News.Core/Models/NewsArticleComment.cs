using System;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.News.Core.Models;

public class NewsArticleComment : AuditableEntity, ICloneable
{
    public string Text { get; set; }

    public object Clone()
    {
        return MemberwiseClone();
    }
}
