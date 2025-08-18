using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.News.Core.Models;
using VirtoCommerce.Platform.Core.GenericCrud;

namespace VirtoCommerce.News.Core.Services;

public interface INewsArticleService : ICrudService<NewsArticle>
{
    Task PublishAsync(IList<string> ids);
    Task UnpublishAsync(IList<string> ids);

    Task ArchiveAsync(IList<string> ids);
    Task UnarchiveAsync(IList<string> ids);

    Task<NewsArticle> CloneAsync(NewsArticle newsArticle);

    Task<IList<string>> GetTagsAsync(string languageCode, bool publishedOnly, DateTime? certainDate);
    IList<string> GetPublishScopes();
}
