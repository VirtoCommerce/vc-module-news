using FluentValidation;
using VirtoCommerce.News.Core.Models;
using VirtoCommerce.News.Data.Models;

namespace VirtoCommerce.News.Data.Validation;

public class NewsArticleValidator : AbstractValidator<NewsArticle>
{
    public NewsArticleValidator()
    {
        RuleFor(na => na.StoreId).NotNull().NotEmpty().MaximumLength(NewsArticleEntity.StoreIdLength);
        RuleFor(na => na.Name).NotNull().NotEmpty().MaximumLength(NewsArticleEntity.NameLength);

        RuleFor(na => na.LocalizedContents).NotEmpty().When(na => na.IsPublished);

        RuleForEach(na => na.LocalizedContents).SetValidator(new NewsArticleLocalizedContentValidator());
    }
}
