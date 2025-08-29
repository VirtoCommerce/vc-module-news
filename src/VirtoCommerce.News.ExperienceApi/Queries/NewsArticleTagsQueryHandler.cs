using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.News.Core.Services;
using VirtoCommerce.Xapi.Core.Infrastructure;

namespace VirtoCommerce.News.ExperienceApi.Queries;

public class NewsArticleTagsQueryHandler(INewsArticleService newsArticleService) : IQueryHandler<NewsArticleTagsQuery, IList<string>>
{
    public async Task<IList<string>> Handle(NewsArticleTagsQuery request, CancellationToken cancellationToken)
    {
        return await newsArticleService.GetTagsAsync(request.LanguageCode, true, null);
    }
}
