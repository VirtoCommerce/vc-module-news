using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.Seo.Core.Models;

namespace VirtoCommerce.News.Core.Services;

public interface INewsArticleSeoService
{
    Task<IList<SeoInfo>> FindActiveSeoAsync(string[] linkSegments, string storeId, string languageCode);
}
