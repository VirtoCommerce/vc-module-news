using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using VirtoCommerce.News.Core.Models;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Domain;
using VirtoCommerce.Platform.Core.Extensions;
using static VirtoCommerce.Platform.Data.Infrastructure.DbContextBase;

namespace VirtoCommerce.News.Data.Models;

public class NewsArticleEntity : AuditableEntity, IDataEntity<NewsArticleEntity, NewsArticle>
{
    public const int StoreIdLength = IdLength;
    public const int NameLength = Length1024;

    [Required]
    [StringLength(StoreIdLength)]
    public string StoreId { get; set; }

    [Required]
    [StringLength(NameLength)]
    public string Name { get; set; }

    public bool IsPublished { get; set; }
    private bool? _isPublishedValue;

    public DateTime? PublishDate { get; set; }

    public bool IsArchived { get; set; }
    private bool? _isArchivedValue;

    public DateTime? ArchiveDate { get; set; }

    public bool IsSharingAllowed { get; set; }

    [StringLength(IdLength)]
    public string AuthorId { get; set; }

    public virtual ObservableCollection<NewsArticleLocalizedContentEntity> LocalizedContents { get; set; } = new NullCollection<NewsArticleLocalizedContentEntity>();

    public virtual ObservableCollection<SeoInfoEntity> SeoInfos { get; set; } = new NullCollection<SeoInfoEntity>();

    public virtual ObservableCollection<NewsArticleUserGroupEntity> UserGroups { get; set; } = new NullCollection<NewsArticleUserGroupEntity>();

    public virtual NewsArticleAuthorEntity Author { get; set; }

    public virtual ObservableCollection<NewsArticleTagEntity> Tags { get; set; } = new NullCollection<NewsArticleTagEntity>();

    public virtual ObservableCollection<NewsArticleCommentEntity> Comments { get; set; } = new NullCollection<NewsArticleCommentEntity>();

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
        model.PublishDate = PublishDate;
        model.ArchiveDate = ArchiveDate;
        model.IsPublished = IsPublished;
        model.IsArchived = IsArchived;
        model.IsSharingAllowed = IsSharingAllowed;

        model.LocalizedContents = LocalizedContents.Select(x => x.ToModel()).ToList();
        model.SeoInfos = SeoInfos.Select(x => x.ToModel()).ToList();
        model.UserGroups = UserGroups.OrderBy(x => x.Group).Select(x => x.Group).ToList();
        model.Author = Author?.ToModel(new NewsArticleAuthor());
        model.Tags = Tags.OrderBy(x => x.Tag).Select(x => x.Tag).ToList();
        model.Comments = Comments.Select(x => x.ToModel()).ToList();

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
        PublishDate = model.PublishDate;
        ArchiveDate = model.ArchiveDate;
        IsSharingAllowed = model.IsSharingAllowed;

        if (model.IsPublishedValue.HasValue)
        {
            IsPublished = model.IsPublishedValue.Value;
            _isPublishedValue = model.IsPublishedValue;
        }

        if (model.IsArchivedValue.HasValue)
        {
            IsArchived = model.IsArchivedValue.Value;
            _isArchivedValue = model.IsArchivedValue;
        }

        if (model.LocalizedContents != null)
        {
            LocalizedContents = new ObservableCollection<NewsArticleLocalizedContentEntity>(model.LocalizedContents.Select(x => AbstractTypeFactory<NewsArticleLocalizedContentEntity>.TryCreateInstance().FromModel(x, pkMap)));
        }

        if (model.SeoInfos != null)
        {
            SeoInfos = new ObservableCollection<SeoInfoEntity>(model.SeoInfos.Select(x => AbstractTypeFactory<SeoInfoEntity>.TryCreateInstance().FromModel(x, pkMap)));
        }

        if (model.UserGroups != null)
        {
            UserGroups = [];

            foreach (var group in model.UserGroups)
            {
                var userGroupEntity = AbstractTypeFactory<NewsArticleUserGroupEntity>.TryCreateInstance();
                userGroupEntity.Group = group;
                userGroupEntity.NewsArticleId = model.Id;

                UserGroups.Add(userGroupEntity);
            }
        }

        if (model.Author != null)
        {
            Author = AbstractTypeFactory<NewsArticleAuthorEntity>.TryCreateInstance().FromModel(model.Author, pkMap);
        }

        if (model.Tags != null)
        {
            Tags = [];

            foreach (var tag in model.Tags)
            {
                var tagEntity = AbstractTypeFactory<NewsArticleTagEntity>.TryCreateInstance();
                tagEntity.Tag = tag;
                tagEntity.NewsArticleId = model.Id;

                Tags.Add(tagEntity);
            }
        }

        if (model.Comments != null)
        {
            Comments = new ObservableCollection<NewsArticleCommentEntity>(model.Comments.Select(x => AbstractTypeFactory<NewsArticleCommentEntity>.TryCreateInstance().FromModel(x, pkMap)));
        }

        return this;
    }

    public void Patch(NewsArticleEntity target)
    {
        ArgumentNullException.ThrowIfNull(target);

        target.StoreId = StoreId;
        target.Name = Name;
        target.PublishDate = PublishDate;
        target.ArchiveDate = ArchiveDate;
        target.IsSharingAllowed = IsSharingAllowed;

        if (_isPublishedValue.HasValue)
        {
            target.IsPublished = _isPublishedValue.Value;
        }

        if (_isArchivedValue.HasValue)
        {
            target.IsArchived = _isArchivedValue.Value;
        }

        if (!LocalizedContents.IsNullCollection())
        {
            LocalizedContents.Patch(target.LocalizedContents, (sourceContent, targetContent) => sourceContent.Patch(targetContent));
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

        if (Author != null)
        {
            Author.Patch(target.Author);
        }

        if (!Tags.IsNullCollection())
        {
            var tagComparer = AnonymousComparer.Create((NewsArticleTagEntity x) => x.Tag);
            Tags.Patch(target.Tags, tagComparer, (sourceTag, targetTag) => targetTag.Tag = sourceTag.Tag);
        }

        if (!Comments.IsNullCollection())
        {
            Comments.Patch(target.Comments, (sourceComment, targetComment) => targetComment.Text = sourceComment.Text);
        }
    }
}
