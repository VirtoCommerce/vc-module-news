using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using VirtoCommerce.News.Core.Models;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Domain;
using static VirtoCommerce.Platform.Data.Infrastructure.DbContextBase;

namespace VirtoCommerce.News.Data.Models;

public class NewsArticleAuthorEntity : AuditableEntity, IDataEntity<NewsArticleAuthorEntity, NewsArticleAuthor>
{
    [Required]
    [StringLength(Length1024)]
    public string Name { get; set; }

    [StringLength(UrlLength)]
    public string PhotoUrl { get; set; }

    public virtual ObservableCollection<NewsArticleEntity> NewsArticles { get; set; } = new NullCollection<NewsArticleEntity>();

    public NewsArticleAuthor ToModel(NewsArticleAuthor model)
    {
        ArgumentNullException.ThrowIfNull(model);

        model.Id = Id;
        model.CreatedDate = CreatedDate;
        model.ModifiedDate = ModifiedDate;
        model.CreatedBy = CreatedBy;
        model.ModifiedBy = ModifiedBy;

        model.Name = Name;
        model.PhotoUrl = PhotoUrl;

        return model;
    }

    public NewsArticleAuthorEntity FromModel(NewsArticleAuthor model, PrimaryKeyResolvingMap pkMap)
    {
        ArgumentNullException.ThrowIfNull(model);

        pkMap.AddPair(model, this);

        Id = model.Id;
        CreatedDate = model.CreatedDate;
        ModifiedDate = model.ModifiedDate;
        CreatedBy = model.CreatedBy;
        ModifiedBy = model.ModifiedBy;

        Name = model.Name;
        PhotoUrl = model.PhotoUrl;

        return this;
    }

    public void Patch(NewsArticleAuthorEntity target)
    {
        ArgumentNullException.ThrowIfNull(target);

        target.Name = Name;
        target.PhotoUrl = PhotoUrl;
    }
}
