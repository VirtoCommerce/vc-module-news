using System.Collections.Generic;
using VirtoCommerce.News.Core.Models;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.News.Core.Events;

public class NewsArticleChangedEvent : GenericChangedEntryEvent<NewsArticle>
{
    public NewsArticleChangedEvent(IEnumerable<GenericChangedEntry<NewsArticle>> changedEntries)
        : base(changedEntries)
    {
    }
}
