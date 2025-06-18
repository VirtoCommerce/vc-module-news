using System.Threading.Tasks;
using GraphQL;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using VirtoCommerce.News.ExperienceApi.Authorization;
using VirtoCommerce.News.ExperienceApi.Models;
using VirtoCommerce.News.ExperienceApi.Schemas;
using VirtoCommerce.Xapi.Core.BaseQueries;

namespace VirtoCommerce.News.ExperienceApi.Queries;

public class NewsArticlesContentQueryBuilder : SearchQueryBuilder<NewsArticlesContentQuery, NewsArticleContentSearchResult, NewsArticleContent, NewsArticleContentType>
{
    protected override string Name => "newsArticles";

    public NewsArticlesContentQueryBuilder(IMediator mediator, IAuthorizationService authorizationService) : base(mediator, authorizationService)
    {
    }

    protected override async Task BeforeMediatorSend(IResolveFieldContext<object> context, NewsArticlesContentQuery request)
    {
        await base.BeforeMediatorSend(context, request);
        await Authorize(context, request, new NewsArticleContentAuthorizationRequirement());
    }
}
