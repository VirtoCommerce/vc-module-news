using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.News.Core.Models;
using VirtoCommerce.News.Data.Repositories;
using VirtoCommerce.Platform.Core.ChangeLog;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.SearchModule.Core.Model;
using VirtoCommerce.SearchModule.Core.Services;
using VirtoCommerce.Seo.Core.Models;

namespace VirtoCommerce.News.Data.Search.Indexed;

public class NewsChangesProvider(
    Func<INewsArticleRepository> repositoryFactory,
    IChangeLogSearchService changeLogSearchService) : IIndexDocumentChangesProvider
{
    private readonly Func<INewsArticleRepository> _repositoryFactory = repositoryFactory ?? throw new ArgumentNullException(nameof(repositoryFactory));
    private readonly IChangeLogSearchService _changeLogSearchService = changeLogSearchService ?? throw new ArgumentNullException(nameof(changeLogSearchService));

    private static readonly string[] _objectTypes = [nameof(NewsArticle), nameof(NewsArticleLocalizedContent), nameof(NewsArticleLocalizedTag), nameof(SeoInfo)];

    public async Task<long> GetTotalChangesCountAsync(DateTime? startDate, DateTime? endDate)
    {
        if (startDate == null && endDate == null)
        {
            using var repository = _repositoryFactory();
            return await repository.NewsArticles.LongCountAsync();
        }

        var criteria = new ChangeLogSearchCriteria
        {
            ObjectTypes = _objectTypes,
            StartDate = startDate,
            EndDate = endDate,
        };

        var result = await _changeLogSearchService.SearchAsync(criteria);
        return result.TotalCount;
    }

    public async Task<IList<IndexDocumentChange>> GetChangesAsync(DateTime? startDate, DateTime? endDate, long skip, long take)
    {
        var allChanges = new List<IndexDocumentChange>();

        if (startDate == null && endDate == null)
        {
            return GetChangesFromRepository(skip, take);
        }

        var changesFromLog = await GetChangesFromOperationLog(startDate, endDate, skip, take);
        var articleIds = await GetArticleIdsFromChanges(changesFromLog);

        foreach (var change in changesFromLog)
        {
            var changeType = change.OperationType == EntryState.Deleted ? IndexDocumentChangeType.Deleted : IndexDocumentChangeType.Modified;

            string documentId;
            if (change.ObjectType == nameof(NewsArticle))
            {
                documentId = change.ObjectId;
            }
            else
            {
                articleIds.TryGetValue(change.ObjectId, out documentId);
            }

            if (documentId != null)
            {
                allChanges.Add(new IndexDocumentChange
                {
                    DocumentId = documentId,
                    ChangeType = changeType,
                    ChangeDate = change.ModifiedDate ?? DateTime.UtcNow
                });
            }
        }

        return allChanges.DistinctBy(x => new { x.DocumentId, x.ChangeType }).ToList();
    }

    private IList<IndexDocumentChange> GetChangesFromRepository(long skip, long take)
    {
        using var repository = _repositoryFactory();
        var ids = repository.NewsArticles
            .OrderBy(x => x.CreatedDate)
            .Select(x => x.Id)
            .Skip((int)skip)
            .Take((int)take)
            .ToArray();

        return ids.Select(id => new IndexDocumentChange
        {
            DocumentId = id,
            ChangeType = IndexDocumentChangeType.Modified,
            ChangeDate = DateTime.UtcNow
        }).ToList();
    }

    private async Task<IList<OperationLog>> GetChangesFromOperationLog(DateTime? startDate, DateTime? endDate, long skip, long take)
    {
        var criteria = new ChangeLogSearchCriteria
        {
            ObjectTypes = _objectTypes,
            StartDate = startDate,
            EndDate = endDate,
            Skip = (int)skip,
            Take = (int)take,
        };

        var result = await _changeLogSearchService.SearchAsync(criteria);
        return result.Results;
    }

    private async Task<IDictionary<string, string>> GetArticleIdsFromChanges(IEnumerable<OperationLog> changes)
    {
        var result = new Dictionary<string, string>();
        var groupedChanges = changes.GroupBy(c => c.ObjectType);

        var localizedContentIds = new List<string>();
        var localizedTagIds = new List<string>();
        var seoInfoIds = new List<string>();

        foreach (var group in groupedChanges)
        {
            switch (group.Key)
            {
                case nameof(NewsArticleLocalizedContent):
                    localizedContentIds.AddRange(group.Select(c => c.ObjectId));
                break;
                case nameof(NewsArticleLocalizedTag):
                    localizedTagIds.AddRange(group.Select(c => c.ObjectId));
                break;
                case nameof(SeoInfo):
                    seoInfoIds.AddRange(group.Select(c => c.ObjectId));
                break;
            }
        }

        if (localizedContentIds.Count == 0 &&
            localizedTagIds.Count == 0 &&
            seoInfoIds.Count == 0)
        {
            return result;
        }

        using var repository = _repositoryFactory();

        if (localizedContentIds.Count != 0)
        {
            var articleIdsFromContent = await repository.GetArticleIdsByContentIdsAsync(localizedContentIds);
            foreach (var (key, value) in articleIdsFromContent) { result[key] = value; }
        }

        if (localizedTagIds.Count != 0)
        {
            var articleIdsFromTags = await repository.GetArticleIdsByTagIdsAsync(localizedTagIds);
            foreach (var (key, value) in articleIdsFromTags) { result[key] = value; }
        }

        if (seoInfoIds.Count != 0)
        {
            var articleIdsFromSeo = await repository.GetArticleIdsBySeoInfoIdsAsync(seoInfoIds);
            foreach (var (key, value) in articleIdsFromSeo) { result[key] = value; }
        }

        return result;
    }
}
