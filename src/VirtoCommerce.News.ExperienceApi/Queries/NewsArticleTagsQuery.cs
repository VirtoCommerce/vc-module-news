using System.Collections.Generic;
using GraphQL;
using GraphQL.Types;
using VirtoCommerce.Xapi.Core.BaseQueries;

namespace VirtoCommerce.News.ExperienceApi.Queries;

public class NewsArticleTagsQuery : Query<IList<string>>
{
    public string LanguageCode { get; set; }

    public override IEnumerable<QueryArgument> GetArguments()
    {
        yield return Argument<NonNullGraphType<StringGraphType>>(nameof(LanguageCode));
    }

    public override void Map(IResolveFieldContext context)
    {
        LanguageCode = context.GetArgument<string>(nameof(LanguageCode));
    }
}
