using System.Collections.Generic;
using GraphQL;
using GraphQL.Types;
using VirtoCommerce.News.Core.Models;
using VirtoCommerce.Xapi.Core.BaseQueries;

namespace VirtoCommerce.News.ExperienceApi.Queries;

public class NewsArticleContentQuery : Query<NewsArticle>
{
    public string Id { get; set; }
    public string StoreId { get; set; }
    public string LanguageCode { get; set; }

    public override IEnumerable<QueryArgument> GetArguments()
    {
        yield return Argument<NonNullGraphType<StringGraphType>>(nameof(Id));
        yield return Argument<NonNullGraphType<StringGraphType>>(nameof(StoreId));
        yield return Argument<NonNullGraphType<StringGraphType>>(nameof(LanguageCode));
    }

    public override void Map(IResolveFieldContext context)
    {
        Id = context.GetArgument<string>(nameof(Id));
        StoreId = context.GetArgument<string>(nameof(StoreId));
        LanguageCode = context.GetArgument<string>(nameof(LanguageCode));
    }
}
