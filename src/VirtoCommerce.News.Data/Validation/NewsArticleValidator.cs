using FluentValidation;
using VirtoCommerce.News.Core.Models;

namespace VirtoCommerce.News.Data.Validation;

public class NewsArticleValidator : AbstractValidator<NewsArticle>
{
    public NewsArticleValidator()
    {
        RuleFor(na => na.StoreId).NotNull().NotEmpty().MaximumLength(128);
        RuleFor(na => na.Name).NotNull().NotEmpty().MaximumLength(1024);

        RuleFor(na => na.LocalizedContents).NotEmpty().When(na => na.IsPublished);

        RuleForEach(na => na.LocalizedContents).SetValidator(new NewsArticleLocalizedContentValidator());
    }
}
