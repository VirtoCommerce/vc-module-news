using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.News.Core.Events;
using VirtoCommerce.News.Core.Models;
using VirtoCommerce.News.Core.Services;
using VirtoCommerce.News.Data.Models;
using VirtoCommerce.News.Data.Repositories;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Data.GenericCrud;

namespace VirtoCommerce.News.Data.Services;

public class NewsArticleService : CrudService<NewsArticle, NewsArticleEntity, NewsArticleChangingEvent, NewsArticleChangedEvent>, INewsArticleService
{
    public NewsArticleService(
        Func<NewsArticleRepository> repositoryFactory,
        IPlatformMemoryCache platformMemoryCache,
        IEventPublisher eventPublisher)
        : base(repositoryFactory, platformMemoryCache, eventPublisher)
    {
    }

    protected override Task<IList<NewsArticleEntity>> LoadEntities(IRepository repository, IList<string> ids, string responseGroup)
    {
        return ((NewsArticleRepository)repository).GetNewsArticlesByIdsAsync(ids);
    }
}
