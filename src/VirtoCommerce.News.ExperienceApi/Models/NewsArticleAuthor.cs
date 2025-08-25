using VirtoCommerce.CustomerModule.Core.Model;

namespace VirtoCommerce.News.ExperienceApi.Models;

public class NewsArticleAuthor
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string IconUrl { get; set; }

    public virtual NewsArticleAuthor FromMember(Member member)
    {
        Id = member.Id;
        Name = member.Name;
        IconUrl = member.IconUrl;

        return this;
    }
}
