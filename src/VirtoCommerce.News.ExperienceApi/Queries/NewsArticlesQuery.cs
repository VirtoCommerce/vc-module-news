using System;
using System.Collections.Generic;
using GraphQL;
using GraphQL.Types;
using VirtoCommerce.News.Core.Models;
using VirtoCommerce.Xapi.Core.BaseQueries;

namespace VirtoCommerce.News.ExperienceApi.Queries;

public class NewsArticlesQuery : SearchQuery<NewsArticleSearchResult>
{
    public string StoreId { get; set; }
    public string LanguageCode { get; set; }
    public string UserId { get; set; }
    public string AuthorId { get; set; }
    public IList<string> Tags { get; set; } = Array.Empty<string>();

    public override IEnumerable<QueryArgument> GetArguments()
    {
        foreach (var argument in base.GetArguments())
        {
            yield return argument;
        }

        yield return Argument<NonNullGraphType<StringGraphType>>(nameof(StoreId));
        yield return Argument<NonNullGraphType<StringGraphType>>(nameof(LanguageCode));
        yield return Argument<StringGraphType>(nameof(UserId));
        yield return Argument<StringGraphType>(nameof(AuthorId));
        yield return Argument<ListGraphType<StringGraphType>>(nameof(Tags));
    }

    public override void Map(IResolveFieldContext context)
    {
        base.Map(context);

        StoreId = context.GetArgument<string>(nameof(StoreId));
        LanguageCode = context.GetArgument<string>(nameof(LanguageCode));
        UserId = context.GetArgument<string>(nameof(UserId));
        AuthorId = context.GetArgument<string>(nameof(AuthorId));
        Tags = context.GetArgument<IList<string>>(nameof(Tags));
    }
}
