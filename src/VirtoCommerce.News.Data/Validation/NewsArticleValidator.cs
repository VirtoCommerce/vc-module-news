using FluentValidation;
using VirtoCommerce.News.Core.Models;
using VirtoCommerce.News.Data.Models;

namespace VirtoCommerce.News.Data.Validation;

public class NewsArticleValidator : AbstractValidator<NewsArticle>
{
    public NewsArticleValidator(AbstractValidator<NewsArticleLocalizedContent> newsArticleLocalizedContentValidator)
    {
        RuleFor(x => x.StoreId).NotNull().NotEmpty().MaximumLength(NewsArticleEntity.StoreIdLength);
        RuleFor(x => x.Name).NotNull().NotEmpty().MaximumLength(NewsArticleEntity.NameLength);

        RuleFor(x => x.LocalizedContents).NotEmpty().When(x => x.IsPublished);

        RuleForEach(x => x.LocalizedContents).SetValidator(newsArticleLocalizedContentValidator);
    }
}
