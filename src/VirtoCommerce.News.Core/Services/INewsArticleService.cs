using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.News.Core.Models;
using VirtoCommerce.Platform.Core.GenericCrud;

namespace VirtoCommerce.News.Core.Services;

public interface INewsArticleService : ICrudService<NewsArticle>
{
    Task PublishAsync(IList<string> ids);
    Task UnpublishAsync(IList<string> ids);
}
