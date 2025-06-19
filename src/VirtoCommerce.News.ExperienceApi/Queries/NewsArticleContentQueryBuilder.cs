using System.Threading.Tasks;
using GraphQL;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using VirtoCommerce.News.Core.Models;
using VirtoCommerce.News.ExperienceApi.Schemas;
using VirtoCommerce.Xapi.Core.BaseQueries;

namespace VirtoCommerce.News.ExperienceApi.Queries;

public class NewsArticleContentQueryBuilder : QueryBuilder<NewsArticleContentQuery, NewsArticle, NewsArticleContentType>
{
    protected override string Name => "newsArticleContent";

    public NewsArticleContentQueryBuilder(IMediator mediator, IAuthorizationService authorizationService) : base(mediator, authorizationService)
    {
    }

    protected override async Task BeforeMediatorSend(IResolveFieldContext<object> context, NewsArticleContentQuery request)
    {
        await base.BeforeMediatorSend(context, request);
        //TODO: uncomment
        //await Authorize(context, request, new NewsArticleContentAuthorizationRequirement());
    }
}
