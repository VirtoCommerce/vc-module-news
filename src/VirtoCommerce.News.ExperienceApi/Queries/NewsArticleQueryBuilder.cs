using System;
using System.Threading.Tasks;
using GraphQL;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using VirtoCommerce.News.Core.Models;
using VirtoCommerce.News.ExperienceApi.Schemas;
using VirtoCommerce.Xapi.Core.BaseQueries;
using VirtoCommerce.Xapi.Core.Extensions;

namespace VirtoCommerce.News.ExperienceApi.Queries;

public class NewsArticleQueryBuilder : QueryBuilder<NewsArticleQuery, NewsArticle, NewsArticleContentType>
{
    protected override string Name => "newsArticle";

    public NewsArticleQueryBuilder(IAuthorizationService authorizationService)
        : base(authorizationService)
    {
    }

    [Obsolete("Use the constructor without IMediator. The mediator is resolved from context.RequestServices per request.", DiagnosticId = "VC0015", UrlFormat = "https://docs.virtocommerce.org/products/products-virto3-versions")]
    public NewsArticleQueryBuilder(IMediator mediator, IAuthorizationService authorizationService)
        : this(authorizationService)
    {
    }

    protected override async Task BeforeMediatorSend(IResolveFieldContext<object> context, NewsArticleQuery request)
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
