using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using VirtoCommerce.News.Core.Models;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Domain;
using VirtoCommerce.Platform.Data.Infrastructure;

namespace VirtoCommerce.News.Data.Models;

public class NewsArticleEntity : AuditableEntity, IDataEntity<NewsArticleEntity, NewsArticle>
{
    [Required]
    [StringLength(DbContextBase.IdLength)]//Q: required?
    public string StoreId { get; set; }

    [Required]
    [StringLength(1024)]
    public string Name { get; set; }

    [Required]
    public bool IsPublished { get; set; }

    public DateTime? PublishDate { get; set; }

    public virtual IList<NewsArticleLocalizedContentEntity> LocalizedContents { get; set; } = new NullCollection<NewsArticleLocalizedContentEntity>();

    public NewsArticle ToModel(NewsArticle model)
    {
        ArgumentNullException.ThrowIfNull(model);

        model.Id = Id;
        model.CreatedDate = CreatedDate;
        model.ModifiedDate = ModifiedDate;
        model.CreatedBy = CreatedBy;
        model.ModifiedBy = ModifiedBy;

        model.StoreId = StoreId;
        model.Name = Name;
        model.SetIsPublished(IsPublished);
        model.PublishDate = PublishDate;

        model.LocalizedContents = LocalizedContents.Select(lc => lc.ToModel(AbstractTypeFactory<NewsArticleLocalizedContent>.TryCreateInstance())).ToList();

        return model;
    }

    public NewsArticleEntity FromModel(NewsArticle model, PrimaryKeyResolvingMap pkMap)
    {
        ArgumentNullException.ThrowIfNull(model);

        pkMap.AddPair(model, this);

        Id = model.Id;
        CreatedDate = model.CreatedDate;
        ModifiedDate = model.ModifiedDate;
        CreatedBy = model.CreatedBy;
        ModifiedBy = model.ModifiedBy;

        StoreId = model.StoreId;
        Name = model.Name;
        IsPublished = model.IsPublished;
        PublishDate = model.PublishDate;

        if (model.LocalizedContents != null)
        {
            LocalizedContents = new ObservableCollection<NewsArticleLocalizedContentEntity>(model.LocalizedContents.Select(lc => AbstractTypeFactory<NewsArticleLocalizedContentEntity>.TryCreateInstance().FromModel(lc, pkMap)));
        }

        return this;
    }

    public void Patch(NewsArticleEntity target)
    {
        ArgumentNullException.ThrowIfNull(target);

        target.StoreId = StoreId;
        target.Name = Name;
        target.IsPublished = IsPublished;
        target.PublishDate = PublishDate;

        if (!LocalizedContents.IsNullCollection())
        {
            LocalizedContents.Patch(target.LocalizedContents, (source, target) => source.Patch(target));
        }
    }
}
