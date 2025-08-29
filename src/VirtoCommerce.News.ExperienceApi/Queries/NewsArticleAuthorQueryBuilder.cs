using MediatR;
using Microsoft.AspNetCore.Authorization;
using VirtoCommerce.News.ExperienceApi.Models;
using VirtoCommerce.News.ExperienceApi.Schemas;
using VirtoCommerce.Xapi.Core.BaseQueries;

namespace VirtoCommerce.News.ExperienceApi.Queries;

public class NewsArticleAuthorQueryBuilder : QueryBuilder<NewsArticleAuthorQuery, NewsArticleAuthor, NewsArticleAuthorType>
{
    protected override string Name => "newsArticleAuthor";

    public NewsArticleAuthorQueryBuilder(IMediator mediator, IAuthorizationService authorizationService) : base(mediator, authorizationService)
    {
    }
}
