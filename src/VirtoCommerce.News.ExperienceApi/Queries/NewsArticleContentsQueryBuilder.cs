using System.Threading.Tasks;
using GraphQL;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using VirtoCommerce.News.Core.Models;
using VirtoCommerce.News.ExperienceApi.Schemas;
using VirtoCommerce.Xapi.Core.BaseQueries;

namespace VirtoCommerce.News.ExperienceApi.Queries;

public class NewsArticleContentsQueryBuilder : SearchQueryBuilder<NewsArticleContentsQuery, NewsArticleSearchResult, NewsArticle, NewsArticleContentType>
{
    protected override string Name => "newsArticleContents";

    public NewsArticleContentsQueryBuilder(IMediator mediator, IAuthorizationService authorizationService) : base(mediator, authorizationService)
    {
    }

    protected override async Task BeforeMediatorSend(IResolveFieldContext<object> context, NewsArticleContentsQuery request)
    {
        await base.BeforeMediatorSend(context, request);
        //TODO: uncomment
        //await Authorize(context, request, new NewsArticleContentAuthorizationRequirement());
    }
}
