using VirtoCommerce.News.ExperienceApi.Models;
using VirtoCommerce.Xapi.Core.Schemas;

namespace VirtoCommerce.News.ExperienceApi.Schemas;

public class NewsArticleAuthorType : ExtendableGraphType<NewsArticleAuthor>
{
    public NewsArticleAuthorType()
    {
        Name = "NewsArticleAuthor";

        Field(x => x.Id);
        Field(x => x.Name, nullable: true);
        Field(x => x.IconUrl, nullable: true);
    }
}
