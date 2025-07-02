using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.Seo.Core.Models;
using VirtoCommerce.Seo.Core.Services;

namespace VirtoCommerce.News.Core.Services;

public interface INewsArticleSeoResolver : ISeoResolver
{
    public Task<IList<SeoInfo>> FindActiveSeoAsync(string lastLinkSegment, string storeId, string languageCode);
}
