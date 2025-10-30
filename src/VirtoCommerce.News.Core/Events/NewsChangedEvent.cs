using System;
using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.News.Core.Models;

namespace VirtoCommerce.News.Core.Events
{
    public class NewsChangedEvent() : IEvent
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public int Version { get; set; } = 1;
        public DateTimeOffset TimeStamp { get; set; } = DateTimeOffset.UtcNow;

        public IList<GenericChangedEntry<NewsArticle>> ChangedEntries { get; set; } = Array.Empty<GenericChangedEntry<NewsArticle>>();

        public NewsChangedEvent(IEnumerable<GenericChangedEntry<NewsArticle>> changedEntries) : this()
        {
            ChangedEntries = changedEntries != null
                ? new List<GenericChangedEntry<NewsArticle>>(changedEntries)
                : new List<GenericChangedEntry<NewsArticle>>();
        }
    }

}
