using System;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using VirtoCommerce.News.ExperienceApi.Models;
using VirtoCommerce.News.ExperienceApi.Schemas;
using VirtoCommerce.Xapi.Core.BaseQueries;

namespace VirtoCommerce.News.ExperienceApi.Queries;

public class NewsArticleAuthorQueryBuilder : QueryBuilder<NewsArticleAuthorQuery, NewsArticleAuthor, NewsArticleAuthorType>
{
    protected override string Name => "newsArticleAuthor";

    public NewsArticleAuthorQueryBuilder(IAuthorizationService authorizationService) : base(authorizationService)
    {
    }

    [Obsolete("Use the constructor without IMediator. The mediator is resolved from context.RequestServices per request.", DiagnosticId = "VC0015", UrlFormat = "https://docs.virtocommerce.org/products/products-virto3-versions")]
    public NewsArticleAuthorQueryBuilder(IMediator mediator, IAuthorizationService authorizationService) : this(authorizationService)
    {
    }
}
