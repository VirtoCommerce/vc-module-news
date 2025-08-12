using System;
using System.ComponentModel.DataAnnotations;
using VirtoCommerce.News.Core.Models;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Domain;
using static VirtoCommerce.Platform.Data.Infrastructure.DbContextBase;

namespace VirtoCommerce.News.Data.Models;

public class NewsArticleCommentEntity : AuditableEntity, IDataEntity<NewsArticleCommentEntity, NewsArticleComment>
{
    [Required]
    [StringLength(IdLength)]
    public string NewsArticleId { get; set; }

    [Required]
    public string Text { get; set; }

    public virtual NewsArticleEntity NewsArticle { get; set; }

    public NewsArticleComment ToModel(NewsArticleComment model)
    {
        ArgumentNullException.ThrowIfNull(model);

        model.Id = Id;
        model.CreatedDate = CreatedDate;
        model.ModifiedDate = ModifiedDate;
        model.CreatedBy = CreatedBy;
        model.ModifiedBy = ModifiedBy;

        model.Text = Text;

        return model;
    }

    public NewsArticleCommentEntity FromModel(NewsArticleComment model, PrimaryKeyResolvingMap pkMap)
    {
        ArgumentNullException.ThrowIfNull(model);

        pkMap.AddPair(model, this);

        Id = model.Id;
        CreatedDate = model.CreatedDate;
        ModifiedDate = model.ModifiedDate;
        CreatedBy = model.CreatedBy;
        ModifiedBy = model.ModifiedBy;

        Text = model.Text;

        return this;
    }

    public void Patch(NewsArticleCommentEntity target)
    {
        ArgumentNullException.ThrowIfNull(target);

        target.Text = Text;
    }
}
