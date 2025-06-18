using VirtoCommerce.News.Core.Models;
using VirtoCommerce.Xapi.Core.Schemas;

namespace VirtoCommerce.News.ExperienceApi.Schemas;

public class NewsArticleLocalizedContentType : ExtendableGraphType<NewsArticleLocalizedContent>
{
    public NewsArticleLocalizedContentType()
    {
        Name = "NewsArticleLocalizedContent";

        Field(x => x.Id);
        Field(x => x.Content);
        Field(x => x.ContentPreview);
    }
}
