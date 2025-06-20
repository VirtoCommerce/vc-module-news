using FluentValidation;
using VirtoCommerce.News.Core.Models;

namespace VirtoCommerce.News.Data.Validation;

public class NewsArticleLocalizedContentValidator : AbstractValidator<NewsArticleLocalizedContent>
{
    public NewsArticleLocalizedContentValidator()
    {
        RuleFor(lc => lc.LanguageCode).NotNull().NotEmpty();
        RuleFor(lc => lc.Title).NotNull().NotEmpty().MaximumLength(1024);
        RuleFor(lc => lc.Content).NotNull().NotEmpty();
    }
}
