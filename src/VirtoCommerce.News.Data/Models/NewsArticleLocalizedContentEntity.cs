using System;
using System.ComponentModel.DataAnnotations;
using VirtoCommerce.News.Core.Models;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Domain;
using VirtoCommerce.Platform.Data.Infrastructure;

namespace VirtoCommerce.News.Data.Models;

public class NewsArticleLocalizedContentEntity : AuditableEntity, IDataEntity<NewsArticleLocalizedContentEntity, NewsArticleLocalizedContent>
{
    public const int LanguageCodeLength = DbContextBase.CultureNameLength;
    public const int TitleLength = DbContextBase.Length1024;

    [Required]
    [StringLength(DbContextBase.IdLength)]
    public string NewsArticleId { get; set; }

    [Required]
    [StringLength(LanguageCodeLength)]
    public string LanguageCode { get; set; }

    [Required]
    [StringLength(TitleLength)]
    public string Title { get; set; }

    [Required]
    public string Content { get; set; }

    public string ContentPreview { get; set; }

    public virtual NewsArticleEntity NewsArticle { get; set; }

    public NewsArticleLocalizedContent ToModel(NewsArticleLocalizedContent model)
    {
        ArgumentNullException.ThrowIfNull(model);

        model.Id = Id;
        model.CreatedDate = CreatedDate;
        model.ModifiedDate = ModifiedDate;
        model.CreatedBy = CreatedBy;
        model.ModifiedBy = ModifiedBy;

        model.NewsArticleId = NewsArticleId;
        model.LanguageCode = LanguageCode;
        model.Title = Title;
        model.Content = Content;
        model.ContentPreview = ContentPreview;

        return model;
    }

    public NewsArticleLocalizedContentEntity FromModel(NewsArticleLocalizedContent model, PrimaryKeyResolvingMap pkMap)
    {
        ArgumentNullException.ThrowIfNull(model);

        pkMap.AddPair(model, this);

        Id = model.Id;
        CreatedDate = model.CreatedDate;
        ModifiedDate = model.ModifiedDate;
        CreatedBy = model.CreatedBy;
        ModifiedBy = model.ModifiedBy;

        LanguageCode = model.LanguageCode;
        Title = model.Title;
        Content = model.Content;
        ContentPreview = model.ContentPreview;

        return this;
    }

    public void Patch(NewsArticleLocalizedContentEntity target)
    {
        ArgumentNullException.ThrowIfNull(target);

        target.LanguageCode = LanguageCode;
        target.Title = Title;
        target.Content = Content;
        target.ContentPreview = ContentPreview;
    }
}
