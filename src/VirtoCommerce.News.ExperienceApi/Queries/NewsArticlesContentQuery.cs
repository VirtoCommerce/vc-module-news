using System.Collections.Generic;
using GraphQL;
using GraphQL.Types;
using VirtoCommerce.News.ExperienceApi.Models;
using VirtoCommerce.Xapi.Core.BaseQueries;

namespace VirtoCommerce.News.ExperienceApi.Queries;

public class NewsArticlesContentQuery : SearchQuery<NewsArticleContentSearchResult>
{
    public string Id { get; set; }
    public string LanguageCode { get; set; }

    public override IEnumerable<QueryArgument> GetArguments()
    {
        foreach (var argument in base.GetArguments())
        {
            yield return argument;
        }
        yield return Argument<StringGraphType>(nameof(Id));
        yield return Argument<NonNullGraphType<StringGraphType>>(nameof(LanguageCode));
    }

    public override void Map(IResolveFieldContext context)
    {
        base.Map(context);

        Id = context.GetArgument<string>(nameof(Id));
        LanguageCode = context.GetArgument<string>(nameof(LanguageCode));
    }
}
