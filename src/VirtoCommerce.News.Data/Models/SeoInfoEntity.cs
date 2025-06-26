using System.ComponentModel.DataAnnotations;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Domain;
using VirtoCommerce.Seo.Core.Models;
using static VirtoCommerce.Platform.Data.Infrastructure.DbContextBase;

namespace VirtoCommerce.News.Data.Models;

public class SeoInfoEntity : AuditableEntity, IDataEntity<SeoInfoEntity, SeoInfo>
{
    [Required]
    [StringLength(IdLength)]
    public string NewsArticleId { get; set; }

    [StringLength(IdLength)]
    public string StoreId { get; set; }

    [StringLength(Length256)]
    [Required]
    public string Keyword { get; set; }

    public bool IsActive { get; set; }

    [StringLength(CultureNameLength)]
    public string LanguageCode { get; set; }

    [StringLength(Length256)]
    public string Title { get; set; }

    [StringLength(Length1024)]
    public string MetaDescription { get; set; }

    [StringLength(Length256)]
    public string MetaKeywords { get; set; }

    [StringLength(Length256)]
    public string ImageAltDescription { get; set; }

    public virtual NewsArticleEntity NewsArticle { get; set; }

    public virtual SeoInfo ToModel(SeoInfo seoInfo)
    {
        seoInfo.Id = Id;
        seoInfo.CreatedBy = CreatedBy;
        seoInfo.CreatedDate = CreatedDate;
        seoInfo.ModifiedBy = ModifiedBy;
        seoInfo.ModifiedDate = ModifiedDate;

        seoInfo.LanguageCode = LanguageCode;
        seoInfo.SemanticUrl = Keyword;
        seoInfo.PageTitle = Title;
        seoInfo.ImageAltDescription = ImageAltDescription;
        seoInfo.IsActive = IsActive;
        seoInfo.MetaDescription = MetaDescription;
        seoInfo.MetaKeywords = MetaKeywords;
        seoInfo.ObjectId = NewsArticleId;
        seoInfo.ObjectType = nameof(NewsArticle);
        seoInfo.StoreId = StoreId;

        return seoInfo;
    }

    public virtual SeoInfoEntity FromModel(SeoInfo seoInfo, PrimaryKeyResolvingMap pkMap)
    {
        pkMap.AddPair(seoInfo, this);

        Id = seoInfo.Id;
        CreatedBy = seoInfo.CreatedBy;
        CreatedDate = seoInfo.CreatedDate;
        ModifiedBy = seoInfo.ModifiedBy;
        ModifiedDate = seoInfo.ModifiedDate;

        LanguageCode = seoInfo.LanguageCode;
        Keyword = seoInfo.SemanticUrl;
        Title = seoInfo.PageTitle;
        ImageAltDescription = seoInfo.ImageAltDescription;
        IsActive = seoInfo.IsActive;
        MetaDescription = seoInfo.MetaDescription;
        MetaKeywords = seoInfo.MetaKeywords;
        StoreId = seoInfo.StoreId;

        return this;
    }

    public virtual void Patch(SeoInfoEntity target)
    {
        target.LanguageCode = LanguageCode;
        target.Keyword = Keyword;
        target.Title = Title;
        target.ImageAltDescription = ImageAltDescription;
        target.IsActive = IsActive;
        target.MetaDescription = MetaDescription;
        target.MetaKeywords = MetaKeywords;
        target.StoreId = StoreId;
    }
}
