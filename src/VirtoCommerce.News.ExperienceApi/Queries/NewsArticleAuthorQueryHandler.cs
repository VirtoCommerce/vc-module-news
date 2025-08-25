using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.CustomerModule.Core.Services;
using VirtoCommerce.News.ExperienceApi.Models;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Xapi.Core.Infrastructure;

namespace VirtoCommerce.News.ExperienceApi.Queries;

public class NewsArticleAuthorQueryHandler(IMemberService memberService) : IQueryHandler<NewsArticleAuthorQuery, NewsArticleAuthor>
{
    public virtual async Task<NewsArticleAuthor> Handle(NewsArticleAuthorQuery request, CancellationToken cancellationToken)
    {
        var member = await memberService.GetByIdAsync(request.AuthorId);

        return AbstractTypeFactory<NewsArticleAuthor>.TryCreateInstance().FromMember(member);
    }
}
