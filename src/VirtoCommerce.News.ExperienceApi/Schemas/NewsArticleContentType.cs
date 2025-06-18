using VirtoCommerce.News.ExperienceApi.Models;
using VirtoCommerce.Xapi.Core.Schemas;

namespace VirtoCommerce.News.ExperienceApi.Schemas;

public class NewsArticleContentType : ExtendableGraphType<NewsArticleContent>
{
    public NewsArticleContentType()
    {
        Name = "NewsArticleContent";

        Field(x => x.Id);
        Field(x => x.Title);
        Field(x => x.PublishedDate, nullable: true);
        Field(x => x.LocalizedContent);
        Field(x => x.LocalizedContentPreview, nullable: true);
    }
}
