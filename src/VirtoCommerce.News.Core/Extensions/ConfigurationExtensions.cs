using Microsoft.Extensions.Configuration;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.News.Core.Extensions;

public static class ConfigurationExtensions
{
    public static bool IsNewsFullTextSearchEnabled(this IConfiguration configuration)
    {
        var v = configuration["Search:NewsFullTextSearchEnabled"];
        return bool.TryParse(v, out var r) && r;
    }
}
