using System.Threading.Tasks;
using GraphQL;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using VirtoCommerce.News.Core.Models;
using VirtoCommerce.News.ExperienceApi.Schemas;
using VirtoCommerce.Xapi.Core.BaseQueries;
using VirtoCommerce.Xapi.Core.Extensions;

namespace VirtoCommerce.News.ExperienceApi.Queries;

public class NewsArticlesQueryBuilder : SearchQueryBuilder<NewsArticlesQuery, NewsArticleSearchResult, NewsArticle, NewsArticleContentType>
{
    protected override string Name => "newsArticles";

    public NewsArticlesQueryBuilder(IMediator mediator, IAuthorizationService authorizationService) : base(mediator, authorizationService)
    {
    }

    protected override async Task BeforeMediatorSend(IResolveFieldContext<object> context, NewsArticlesQuery request)
    {
        await base.BeforeMediatorSend(context, request);
        if (context.User.Identity.IsAuthenticated)
        {
            request.UserId = context.User.GetCurrentUserId();
        }
        else
        {
            request.UserId = null;
        }
    }
}
