using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using VirtoCommerce.News.Core.Models;
using VirtoCommerce.News.Core.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.ExportImport;

namespace VirtoCommerce.News.Data.ExportImport;

public class NewsArticlesExportImport(INewsArticleService newsArticleService, INewsArticleSearchService newsArticleSearchService, JsonSerializer serializer)
{
    private readonly int _batchSize = 50;

    public async Task DoExportAsync(Stream outStream, Action<ExportImportProgressInfo> progressCallback, ICancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var progressInfo = new ExportImportProgressInfo { Description = "News articles exporting..." };
        progressCallback(progressInfo);

        using (var sw = new StreamWriter(outStream, System.Text.Encoding.UTF8))
        using (var writer = new JsonTextWriter(sw))
        {
            await writer.WriteStartObjectAsync();
            await writer.WritePropertyNameAsync("NewsArticles");

            await writer.SerializeArrayWithPagingAsync(
                serializer,
                _batchSize,
                async (skip, take) =>
                {
                    return (GenericSearchResult<NewsArticle>)await newsArticleSearchService.SearchAsync(new NewsArticleSearchCriteria { Skip = skip, Take = take });
                },
                (processedCount, totalCount) =>
                {
                    progressInfo.Description = $"{processedCount} of {totalCount} news articles have been exported";
                    progressCallback(progressInfo);
                },
                cancellationToken);

            await writer.WriteEndObjectAsync();
            await writer.FlushAsync();
        }
    }

    public async Task DoImportAsync(Stream inputStream, Action<ExportImportProgressInfo> progressCallback, ICancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var progressInfo = new ExportImportProgressInfo { Description = "News articles importing..." };
        progressCallback(progressInfo);

        using (var streamReader = new StreamReader(inputStream))
        using (var reader = new JsonTextReader(streamReader))
        {
            while (await reader.ReadAsync())
            {
                if (reader.TokenType != JsonToken.PropertyName)
                {
                    continue;
                }

                if (reader.Value.ToString() == "NewsArticles")
                {
                    await reader.DeserializeArrayWithPagingAsync<NewsArticle>(
                        serializer,
                        _batchSize,
                        async items =>
                        {
                            foreach (var item in items)
                            {
                                item.SetIsPublished(item.IsPublished);
                            }

                            await newsArticleService.SaveChangesAsync(items.ToArray());
                        },
                        processedCount =>
                        {
                            progressInfo.Description = $"{processedCount} news articles have been imported";
                            progressCallback(progressInfo);
                        },
                        cancellationToken);
                }
            }
        }
    }
}
