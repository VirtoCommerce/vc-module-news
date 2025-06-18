using System;

namespace VirtoCommerce.News.ExperienceApi.Models;

public class NewsArticleContent
{
    public string Id { get; set; }
    public DateTime? PublishedDate { get; set; }
    public string Title { get; set; }
    public string LocalizedContent { get; set; }
    public string LocalizedContentPreview { get; set; }
}
