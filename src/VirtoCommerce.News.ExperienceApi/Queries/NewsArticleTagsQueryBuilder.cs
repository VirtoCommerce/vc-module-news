using System.Collections.Generic;
using GraphQL.Types;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using VirtoCommerce.Xapi.Core.BaseQueries;

namespace VirtoCommerce.News.ExperienceApi.Queries;

public class NewsArticleTagsQueryBuilder : QueryBuilder<NewsArticleTagsQuery, IList<string>, ListGraphType<StringGraphType>>
{
    protected override string Name => "newsArticleTags";

    public NewsArticleTagsQueryBuilder(IMediator mediator, IAuthorizationService authorizationService) : base(mediator, authorizationService)
    {
    }
}
