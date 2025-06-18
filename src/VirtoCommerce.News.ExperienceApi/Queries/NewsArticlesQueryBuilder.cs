using MediatR;
using Microsoft.AspNetCore.Authorization;
using VirtoCommerce.News.Core.Models;
using VirtoCommerce.News.ExperienceApi.Schemas;
using VirtoCommerce.Xapi.Core.BaseQueries;

namespace VirtoCommerce.News.ExperienceApi.Queries;

public class NewsArticlesQueryBuilder : SearchQueryBuilder<NewsArticlesQuery, NewsArticleSearchResult, NewsArticle, NewsArticleType>
{
    protected override string Name => "newsArticles";

    public NewsArticlesQueryBuilder(IMediator mediator, IAuthorizationService authorizationService) : base(mediator, authorizationService)
    {
    }
}
