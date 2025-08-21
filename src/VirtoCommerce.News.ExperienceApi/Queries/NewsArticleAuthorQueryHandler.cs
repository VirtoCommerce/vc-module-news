using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.CustomerModule.Core.Services;
using VirtoCommerce.News.ExperienceApi.Models;
using VirtoCommerce.Xapi.Core.Infrastructure;

namespace VirtoCommerce.News.ExperienceApi.Queries;

public class NewsArticleAuthorQueryHandler(IMemberService memberService) : IQueryHandler<NewsArticleAuthorQuery, NewsArticleAuthor>
{
    public async Task<NewsArticleAuthor> Handle(NewsArticleAuthorQuery request, CancellationToken cancellationToken)
    {
        var member = await memberService.GetByIdAsync(request.AuthorId);
        return new NewsArticleAuthor() { Id = member.Id, Name = member.Name, IconUrl = member.IconUrl };
    }
}
