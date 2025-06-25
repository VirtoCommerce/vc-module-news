using FluentValidation;
using VirtoCommerce.News.Core.Models;
using VirtoCommerce.News.Data.Models;

namespace VirtoCommerce.News.Data.Validation;

public class NewsArticleLocalizedContentValidator : AbstractValidator<NewsArticleLocalizedContent>
{
    public NewsArticleLocalizedContentValidator()
    {
        RuleFor(lc => lc.LanguageCode).NotNull().NotEmpty().MaximumLength(NewsArticleLocalizedContentEntity.LanguageCodeLength);
        RuleFor(lc => lc.Title).NotNull().NotEmpty().MaximumLength(NewsArticleLocalizedContentEntity.TitleLength);
        RuleFor(lc => lc.Content).NotNull().NotEmpty();
    }
}
