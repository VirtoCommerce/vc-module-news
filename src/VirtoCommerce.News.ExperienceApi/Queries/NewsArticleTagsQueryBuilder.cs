using System;
using System.Collections.Generic;
using GraphQL.Types;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using VirtoCommerce.Xapi.Core.BaseQueries;

namespace VirtoCommerce.News.ExperienceApi.Queries;

public class NewsArticleTagsQueryBuilder : QueryBuilder<NewsArticleTagsQuery, IList<string>, ListGraphType<StringGraphType>>
{
    protected override string Name => "newsArticleTags";

    public NewsArticleTagsQueryBuilder(IAuthorizationService authorizationService) : base(authorizationService)
    {
    }

    [Obsolete("Use the constructor without IMediator. The mediator is resolved from context.RequestServices per request.", DiagnosticId = "VC0015", UrlFormat = "https://docs.virtocommerce.org/products/products-virto3-versions")]
    public NewsArticleTagsQueryBuilder(IMediator mediator, IAuthorizationService authorizationService) : this(authorizationService)
    {
    }
}
