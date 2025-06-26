using System.Linq;
using GraphQL.Types;
using VirtoCommerce.News.Core.Models;
using VirtoCommerce.Xapi.Core.Schemas;

namespace VirtoCommerce.News.ExperienceApi.Schemas;

public class NewsArticleContentType : ExtendableGraphType<NewsArticle>
{
    public NewsArticleContentType()
    {
        Name = "NewsArticleContent";

        Field(x => x.Id);
        Field(x => x.PublishDate, nullable: true);

        Field<StringGraphType>("title").Resolve(context => context.Source.LocalizedContents.FirstOrDefault()?.Title);
        Field<StringGraphType>("content").Resolve(context => context.Source.LocalizedContents.FirstOrDefault()?.Content);
        Field<StringGraphType>("contentPreview").Resolve(context => context.Source.LocalizedContents.FirstOrDefault()?.ContentPreview);

        ExtendableField<NonNullGraphType<SeoInfoType>>("seoInfo", resolve: context => context.Source.SeoInfos.FirstOrDefault());
    }
}
