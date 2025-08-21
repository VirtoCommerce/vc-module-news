using System.Collections.Generic;
using GraphQL;
using GraphQL.Types;
using VirtoCommerce.News.ExperienceApi.Models;
using VirtoCommerce.Xapi.Core.BaseQueries;

namespace VirtoCommerce.News.ExperienceApi.Queries;

public class NewsArticleAuthorQuery : Query<NewsArticleAuthor>
{
    public string AuthorId { get; set; }

    public override IEnumerable<QueryArgument> GetArguments()
    {
        yield return Argument<NonNullGraphType<StringGraphType>>(nameof(AuthorId));
    }

    public override void Map(IResolveFieldContext context)
    {
        AuthorId = context.GetArgument<string>(nameof(AuthorId));
    }
}
