using System.Collections.Generic;
using GraphQL;
using GraphQL.Types;
using VirtoCommerce.News.Core.Models;
using VirtoCommerce.Xapi.Core.BaseQueries;

namespace VirtoCommerce.News.ExperienceApi.Queries;

public class NewsArticlesQuery : SearchQuery<NewsArticleSearchResult>
{
    public string LanguageCode { get; set; }

    public override IEnumerable<QueryArgument> GetArguments()
    {
        foreach (var argument in base.GetArguments())
        {
            yield return argument;
        }
        yield return Argument<StringGraphType>(nameof(LanguageCode));
    }

    public override void Map(IResolveFieldContext context)
    {
        base.Map(context);

        LanguageCode = context.GetArgument<string>(nameof(LanguageCode));
    }
}
