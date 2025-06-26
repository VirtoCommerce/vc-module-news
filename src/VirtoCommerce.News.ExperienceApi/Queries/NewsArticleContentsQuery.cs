using System.Collections.Generic;
using GraphQL;
using GraphQL.Types;
using VirtoCommerce.News.Core.Models;
using VirtoCommerce.Xapi.Core.BaseQueries;

namespace VirtoCommerce.News.ExperienceApi.Queries;

public class NewsArticleContentsQuery : SearchQuery<NewsArticleSearchResult>
{
    public string StoreId { get; set; }
    public string LanguageCode { get; set; }
    public string UserId { get; set; }

    public override IEnumerable<QueryArgument> GetArguments()
    {
        foreach (var argument in base.GetArguments())
        {
            yield return argument;
        }

        yield return Argument<NonNullGraphType<StringGraphType>>(nameof(StoreId));
        yield return Argument<NonNullGraphType<StringGraphType>>(nameof(LanguageCode));
        yield return Argument<StringGraphType>(nameof(UserId));
    }

    public override void Map(IResolveFieldContext context)
    {
        base.Map(context);

        StoreId = context.GetArgument<string>(nameof(StoreId));
        LanguageCode = context.GetArgument<string>(nameof(LanguageCode));
        UserId = context.GetArgument<string>(nameof(UserId));
    }
}
