using GraphQL.Types;
using VirtoCommerce.News.Core.Models;
using VirtoCommerce.Xapi.Core.Schemas;

namespace VirtoCommerce.News.ExperienceApi.Schemas;

public class NewsArticleType : ExtendableGraphType<NewsArticle>
{
    public NewsArticleType()
    {
        Name = "NewsArticle";

        Field(x => x.Id);
        Field(x => x.CreatedDate);
        Field(x => x.ModifiedDate, nullable: true);
        Field(x => x.Name);
        Field(x => x.IsPublished);
        Field(x => x.PublishDate, nullable: true);
        ExtendableField<ListGraphType<NewsArticleLocalizedContentType>>(nameof(NewsArticle.LocalizedContents), resolve: context => context.Source.LocalizedContents);
    }
}
