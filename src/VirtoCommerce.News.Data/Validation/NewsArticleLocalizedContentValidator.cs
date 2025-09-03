using FluentValidation;
using VirtoCommerce.News.Core.Models;
using VirtoCommerce.News.Data.Models;

namespace VirtoCommerce.News.Data.Validation;

public class NewsArticleLocalizedContentValidator : AbstractValidator<NewsArticleLocalizedContent>
{
    public NewsArticleLocalizedContentValidator()
    {
        RuleFor(x => x.LanguageCode).NotNull().NotEmpty().MaximumLength(NewsArticleLocalizedContentEntity.LanguageCodeLength);
        RuleFor(x => x.Title).NotNull().NotEmpty().MaximumLength(NewsArticleLocalizedContentEntity.TitleLength);
    }
}
