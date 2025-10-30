using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.News.Core.Models;
using VirtoCommerce.News.Core.Services;
using VirtoCommerce.News.Data.Repositories;
using VirtoCommerce.Platform.Core.GenericCrud;
using VirtoCommerce.SearchModule.Core.Services;

namespace VirtoCommerce.News.Data.Services;

public class NewsArticleIndexedSearchService(
    ISearchService<NewsArticleIndexedSearchCriteria, NewsArticleSearchResult, NewsArticle> searchService,
    Func<INewsArticleRepository> repositoryFactory,
    ISearchRequestBuilder searchRequestBuilder)
    : INewsArticleIndexedSearchService
{
    private readonly Func<INewsArticleRepository> _repositoryFactory = repositoryFactory ?? throw new ArgumentNullException(nameof(repositoryFactory));
    private readonly ISearchRequestBuilder _searchRequestBuilder = searchRequestBuilder ?? throw new ArgumentNullException(nameof(searchRequestBuilder));
    private readonly ISearchService<NewsArticleIndexedSearchCriteria, NewsArticleSearchResult, NewsArticle> _searchService = searchService ?? throw new ArgumentNullException(nameof(searchService));

    public async Task<IList<NewsArticle>> GetByIdsAsync(IList<string> ids)
    {
        using var repository = _repositoryFactory();

        var results = await repository.GetNewsArticlesByIdsAsync(ids);

        var newsArticles = results.Select(x => x.ToModel(new NewsArticle())).ToList();

        return newsArticles;
    }

    public async Task<NewsArticleSearchResult> SearchAsync(NewsArticleIndexedSearchCriteria criteria, bool clone = true)
    {
        var searchRequest = await _searchRequestBuilder.BuildRequestAsync(criteria);



    }
}
