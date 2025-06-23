using System.ComponentModel.DataAnnotations;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Data.Infrastructure;

namespace VirtoCommerce.News.Data.Models;

public class NewsArticleUserGroupEntity : Entity
{
    [Required]
    [StringLength(DbContextBase.IdLength)]
    public string NewsArticleId { get; set; }

    [StringLength(64)]
    public string Group { get; set; }

    public virtual NewsArticleEntity NewsArticle { get; set; }
}
