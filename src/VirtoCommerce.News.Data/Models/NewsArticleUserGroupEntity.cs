using System.ComponentModel.DataAnnotations;
using VirtoCommerce.Platform.Core.Common;
using static VirtoCommerce.Platform.Data.Infrastructure.DbContextBase;

namespace VirtoCommerce.News.Data.Models;

public class NewsArticleUserGroupEntity : Entity
{
    [Required]
    [StringLength(IdLength)]
    public string NewsArticleId { get; set; }

    [Required]
    [StringLength(Length64)]
    public string Group { get; set; }

    public virtual NewsArticleEntity NewsArticle { get; set; }
}
