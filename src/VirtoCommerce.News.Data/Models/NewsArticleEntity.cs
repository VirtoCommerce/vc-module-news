using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using VirtoCommerce.News.Core.Models;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Domain;
using VirtoCommerce.Platform.Data.Infrastructure;
using VirtoCommerce.Seo.Core.Models;

namespace VirtoCommerce.News.Data.Models;

public class NewsArticleEntity : AuditableEntity, IDataEntity<NewsArticleEntity, NewsArticle>
{
    public const int StoreIdLength = DbContextBase.IdLength;
    public const int NameLength = DbContextBase.Length1024;

    [Required]
    [StringLength(StoreIdLength)]
    public string StoreId { get; set; }

    [Required]
    [StringLength(NameLength)]
    public string Name { get; set; }

    public bool IsPublished { get; set; }
    private bool? _isPublishedValue;

    public DateTime? PublishDate { get; set; }

    public virtual ObservableCollection<NewsArticleLocalizedContentEntity> LocalizedContents { get; set; } = new NullCollection<NewsArticleLocalizedContentEntity>();

    public virtual ObservableCollection<SeoInfoEntity> SeoInfos { get; set; } = new NullCollection<SeoInfoEntity>();

    public virtual ObservableCollection<NewsArticleUserGroupEntity> UserGroups { get; set; } = new NullCollection<NewsArticleUserGroupEntity>();

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
        model.IsPublished = IsPublished;
        model.PublishDate = PublishDate;

        model.LocalizedContents = LocalizedContents.Select(lc => lc.ToModel(AbstractTypeFactory<NewsArticleLocalizedContent>.TryCreateInstance())).ToList();
        model.SeoInfos = SeoInfos.Select(x => x.ToModel(AbstractTypeFactory<SeoInfo>.TryCreateInstance())).ToList();
        model.UserGroups = UserGroups.OrderBy(x => x.Id).Select(x => x.Group).ToList();

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
        if (model.IsPublishedValue.HasValue)
        {
            IsPublished = model.IsPublishedValue.Value;
            _isPublishedValue = model.IsPublishedValue;
        }

        PublishDate = model.PublishDate;

        if (model.LocalizedContents != null)
        {
            LocalizedContents = new ObservableCollection<NewsArticleLocalizedContentEntity>(model.LocalizedContents.Select(lc => AbstractTypeFactory<NewsArticleLocalizedContentEntity>.TryCreateInstance().FromModel(lc, pkMap)));
        }

        if (model.SeoInfos != null)
        {
            SeoInfos = new ObservableCollection<SeoInfoEntity>(model.SeoInfos.Select(x => AbstractTypeFactory<SeoInfoEntity>.TryCreateInstance().FromModel(x, pkMap)));
        }

        if (model.UserGroups != null)
        {
            UserGroups = new ObservableCollection<NewsArticleUserGroupEntity>();
            foreach (var group in model.UserGroups)
            {
                var userGroupEntity = AbstractTypeFactory<NewsArticleUserGroupEntity>.TryCreateInstance();
                userGroupEntity.Group = group;
                userGroupEntity.NewsArticleId = model.Id;
                UserGroups.Add(userGroupEntity);
            }
        }

        return this;
    }

    public void Patch(NewsArticleEntity target)
    {
        ArgumentNullException.ThrowIfNull(target);

        target.StoreId = StoreId;
        target.Name = Name;

        if (_isPublishedValue.HasValue)
        {
            target.IsPublished = _isPublishedValue.Value;
        }
        target.PublishDate = PublishDate;

        if (!LocalizedContents.IsNullCollection())
        {
            LocalizedContents.Patch(target.LocalizedContents, (source, target) => source.Patch(target));
        }
        if (!SeoInfos.IsNullCollection())
        {
            SeoInfos.Patch(target.SeoInfos, (sourceSeoInfo, targetSeoInfo) => sourceSeoInfo.Patch(targetSeoInfo));
        }
        if (!UserGroups.IsNullCollection())
        {
            var userGroupComparer = AnonymousComparer.Create((NewsArticleUserGroupEntity x) => x.Group);
            UserGroups.Patch(target.UserGroups, userGroupComparer, (sourceGroup, targetGroup) => targetGroup.Group = sourceGroup.Group);
        }
    }
}
