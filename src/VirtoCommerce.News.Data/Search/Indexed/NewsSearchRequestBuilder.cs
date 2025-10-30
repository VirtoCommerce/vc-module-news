using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.News.Core;
using VirtoCommerce.News.Core.Models;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.SearchModule.Core.Extensions;
using VirtoCommerce.SearchModule.Core.Model;
using VirtoCommerce.SearchModule.Core.Services;

namespace VirtoCommerce.News.Data.Search.Indexed;

public class NewsSearchRequestBuilder : ISearchRequestBuilder
{
    public string DocumentType => ModuleConstants.NewsIndexDocumentType;

    public Task<SearchRequest> BuildRequestAsync(SearchCriteriaBase criteria)
    {
        if (criteria is not NewsArticleIndexedSearchCriteria newsCriteria)
        {
            return Task.FromResult<SearchRequest>(null);
        }

        var filters = new List<IFilter>();

        if (!string.IsNullOrEmpty(newsCriteria.StoreId))
        {
            filters.Add(new TermFilter { FieldName = "StoreId", Values = [newsCriteria.StoreId] });
        }

        if (newsCriteria.Status.HasValue)
        {
            var utcNow = newsCriteria.CertainDate ?? DateTime.UtcNow;
            switch (newsCriteria.Status)
            {
                case NewsArticleStatus.Published:
                    filters.Add(new TermFilter { FieldName = "IsPublished", Values = ["true"] });
                    var publishedRange = new RangeFilterValue { Upper = utcNow.ToString("o"), IncludeUpper = true };
                    filters.Add(new RangeFilter { FieldName = "PublishDate", Values = [publishedRange] });
                    break;
                case NewsArticleStatus.Draft:
                    filters.Add(new TermFilter { FieldName = "IsPublished", Values = ["false"] });
                    break;
                case NewsArticleStatus.Archived:
                    filters.Add(new TermFilter { FieldName = "IsArchived", Values = ["true"] });
                    var archivedRange = new RangeFilterValue { Upper = utcNow.ToString("o"), IncludeUpper = true };
                    filters.Add(new RangeFilter { FieldName = "ArchiveDate", Values = [archivedRange] });
                    break;
                case NewsArticleStatus.Scheduled:
                    filters.Add(new TermFilter { FieldName = "IsPublished", Values = ["true"] });
                    var scheduledRange = new RangeFilterValue { Lower = utcNow.ToString("o"), IncludeLower = false };
                    filters.Add(new RangeFilter { FieldName = "PublishDate", Values = [scheduledRange] });
                    break;
            }
        }

        if (newsCriteria.UserGroups != null && newsCriteria.UserGroups.Any())
        {
            filters.Add(new TermFilter { FieldName = "UserGroups", Values = newsCriteria.UserGroups.ToList() });
        }

        if (!string.IsNullOrEmpty(newsCriteria.PublishScope))
        {
            filters.Add(new TermFilter { FieldName = "PublishScope", Values = [newsCriteria.PublishScope] });
        }

        if (!string.IsNullOrEmpty(newsCriteria.AuthorId))
        {
            filters.Add(new TermFilter { FieldName = "AuthorId", Values = [newsCriteria.AuthorId] });
        }

        if (newsCriteria.Tags != null && newsCriteria.Tags.Any())
        {
            filters.Add(new TermFilter { FieldName = "LocalizedTag_Tag", Values = newsCriteria.Tags.ToList() });
        }

        IFilter filter = null;
        if (filters.Count > 1)
        {
            filter = new AndFilter { ChildFilters = filters };
        }
        else if (filters.Any())
        {
            filter = filters.First();
        }

        var keywordBuilder = new StringBuilder();
        if (!string.IsNullOrEmpty(newsCriteria.Keyword))
        {
            keywordBuilder.Append(newsCriteria.Keyword);
        }
        if (!string.IsNullOrEmpty(newsCriteria.ContentKeyword))
        {
            if (keywordBuilder.Length > 0)
            {
                keywordBuilder.Append(' ');
            }
            keywordBuilder.Append(newsCriteria.ContentKeyword);
        }

        var request = new SearchRequest
        {
            SearchKeywords = keywordBuilder.ToString(),
            SearchFields = [IndexDocumentExtensions.ContentFieldName],
            Filter = filter,
            Sorting = [new SortingField("PublishDate", true)],
            Skip = criteria.Skip,
            Take = criteria.Take
        };

        return Task.FromResult(request);
    }
}
