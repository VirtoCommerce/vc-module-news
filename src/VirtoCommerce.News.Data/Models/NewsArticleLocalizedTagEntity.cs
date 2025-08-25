using System;
using System.ComponentModel.DataAnnotations;
using VirtoCommerce.News.Core.Models;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Domain;
using static VirtoCommerce.Platform.Data.Infrastructure.DbContextBase;

namespace VirtoCommerce.News.Data.Models;

public class NewsArticleLocalizedTagEntity : Entity, IDataEntity<NewsArticleLocalizedTagEntity, NewsArticleLocalizedTag>
{
    [Required]
    [StringLength(IdLength)]
    public string NewsArticleId { get; set; }

    [Required]
    [StringLength(CultureNameLength)]
    public string LanguageCode { get; set; }

    [Required]
    [StringLength(Length64)]
    public string Tag { get; set; }

    public virtual NewsArticleEntity NewsArticle { get; set; }

    public NewsArticleLocalizedTag ToModel(NewsArticleLocalizedTag model)
    {
        ArgumentNullException.ThrowIfNull(model);

        model.Id = Id;

        model.NewsArticleId = NewsArticleId;
        model.LanguageCode = LanguageCode;
        model.Tag = Tag;

        return model;
    }

    public NewsArticleLocalizedTagEntity FromModel(NewsArticleLocalizedTag model, PrimaryKeyResolvingMap pkMap)
    {
        ArgumentNullException.ThrowIfNull(model);

        pkMap.AddPair(model, this);

        Id = model.Id;

        LanguageCode = model.LanguageCode;
        Tag = model.Tag;

        return this;
    }

    public void Patch(NewsArticleLocalizedTagEntity target)
    {
        ArgumentNullException.ThrowIfNull(target);

        target.LanguageCode = LanguageCode;
        target.Tag = Tag;
    }
}
