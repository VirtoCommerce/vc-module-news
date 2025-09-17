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

    public NewsArticleQueryBuilder(IMediator mediator, IAuthorizationService authorizationService)
        : base(mediator, authorizationService)
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
