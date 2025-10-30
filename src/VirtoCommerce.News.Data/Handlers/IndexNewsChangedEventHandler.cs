using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.SearchModule.Core.Model;
using VirtoCommerce.News.Core;
using VirtoCommerce.News.Core.Events;
using VirtoCommerce.News.Core.Extensions;
using VirtoCommerce.News.Data.Search.Indexed;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Core.Jobs;
using VirtoCommerce.SearchModule.Core.BackgroundJobs;
using VirtoCommerce.SearchModule.Core.Extensions;

namespace VirtoCommerce.News.Data.Handlers
{
    public class IndexNewsChangedEventHandler(
        ISettingsManager settingsManager,
        IConfiguration configuration,
        IIndexingJobService indexingJobService,
        IEnumerable<IndexDocumentConfiguration> indexingConfigurations
    )
        : IEventHandler<NewsChangedEvent>
    {
        public async Task Handle(NewsChangedEvent message)
        {
            if (!configuration.IsNewsFullTextSearchEnabled() ||
                !await settingsManager.GetValueAsync<bool>(ModuleConstants.Settings.General.EventBasedIndexation))
            {
                return;
            }

            var indexEntries = message?.ChangedEntries
                .Select(x =>
                {
                    var id = x.EntryState == EntryState.Added ? x.NewEntry?.Id : x.OldEntry?.Id;
                    return new IndexEntry
                    {
                        Id = id,
                        EntryState = x.EntryState,
                        Type = ModuleConstants.NewsIndexDocumentType
                    };
                })
                .Where(e => !string.IsNullOrEmpty(e.Id))
                .ToArray() ?? [];


            var builders = indexingConfigurations
                .GetDocumentBuilders(ModuleConstants.NewsIndexDocumentType, typeof(NewsChangesProvider))
                .ToList();

            indexingJobService.EnqueueIndexAndDeleteDocuments(indexEntries, JobPriority.Normal, builders);
        }
    }
}
