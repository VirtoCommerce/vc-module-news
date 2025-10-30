using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.News.Core.Services;
using VirtoCommerce.SearchModule.Core.Extensions;
using VirtoCommerce.SearchModule.Core.Model;
using VirtoCommerce.SearchModule.Core.Services;

namespace VirtoCommerce.News.Data.Search.Indexed;

public class NewsDocumentBuilder(INewsArticleIndexedSearchService newsService)
    : IIndexSchemaBuilder, IIndexDocumentBuilder
{
    private readonly INewsArticleIndexedSearchService _newsService = newsService ?? throw new ArgumentNullException(nameof(newsService));

    public Task BuildSchemaAsync(IndexDocument schema)
    {
        // Main properties of NewsArticle
        schema.AddFilterableString("StoreId");
        schema.AddFilterableStringAndContentString("Name");
        schema.AddFilterableBoolean("IsPublished");
        schema.AddFilterableDateTime("PublishDate");
        schema.AddFilterableBoolean("IsArchived");
        schema.AddFilterableDateTime("ArchiveDate");
        schema.AddFilterableString("AuthorId");
        schema.AddFilterableString("PublishScope");

        // Collection of UserGroups strings.
        // The search engine will handle multiple values for this field automatically.
        schema.AddFilterableString("UserGroups");

        // Properties from LocalizedContents
        schema.AddFilterableStringAndContentString("LocalizedContent_Title");
        schema.AddFilterableStringAndContentString("LocalizedContent_Body");

        // Properties from LocalizedTags
        schema.AddFilterableStringAndContentString("LocalizedTag_Tag");

        // Properties from SeoInfos
        schema.AddFilterableStringAndContentString("SeoInfo_Title");
        schema.AddFilterableStringAndContentString("SeoInfo_MetaDescription");
        schema.AddFilterableStringAndContentString("SeoInfo_MetaKeywords");
        schema.AddFilterableStringAndContentString("SeoInfo_Slug");

        return Task.CompletedTask;
    }

    public async Task<IList<IndexDocument>> GetDocumentsAsync(IList<string> documentIds)
    {
        var articles = await _newsService.GetByIdsAsync(documentIds);
        var documents = new List<IndexDocument>();

        foreach (var article in articles)
        {
            var document = new IndexDocument(article.Id);

            document.AddFilterableString("StoreId", article.StoreId);
            document.AddFilterableStringAndContentString("Name", article.Name);
            document.AddFilterableBoolean("IsPublished", article.IsPublished);
            document.AddFilterableDateTime("PublishDate", article.PublishDate);
            document.AddFilterableBoolean("IsArchived", article.IsArchived);
            document.AddFilterableDateTime("ArchiveDate", article.ArchiveDate);
            document.AddFilterableString("AuthorId", article.AuthorId);
            document.AddFilterableString("PublishScope", article.PublishScope);

            if (article.UserGroups != null)
            {
                foreach (var userGroup in article.UserGroups.Where(g => !string.IsNullOrEmpty(g)))
                {
                    document.AddFilterableString("UserGroups", userGroup);
                }
            }

            if (article.LocalizedContents != null)
            {
                foreach (var content in article.LocalizedContents)
                {
                    if (!string.IsNullOrEmpty(content.Title))
                    {
                        document.AddFilterableStringAndContentString("LocalizedContent_Title", content.Title);
                    }
                    if (!string.IsNullOrEmpty(content.Content))
                    {
                        document.AddFilterableStringAndContentString("LocalizedContent_Body", content.Content);
                    }
                }
            }

            if (article.LocalizedTags != null)
            {
                foreach (var tag in article.LocalizedTags.Select(t => t.Tag).Where(t => !string.IsNullOrEmpty(t)))
                {
                    document.AddFilterableStringAndContentString("LocalizedTag_Tag", tag);
                }
            }

            if (article.SeoInfos != null)
            {
                foreach (var seoInfo in article.SeoInfos)
                {
                    if (!string.IsNullOrEmpty(seoInfo.PageTitle))
                    {
                        document.AddFilterableStringAndContentString("SeoInfo_Title", seoInfo.PageTitle);
                    }
                    if (!string.IsNullOrEmpty(seoInfo.MetaDescription))
                    {
                        document.AddFilterableStringAndContentString("SeoInfo_MetaDescription", seoInfo.MetaDescription);
                    }
                    if (!string.IsNullOrEmpty(seoInfo.MetaKeywords))
                    {
                        document.AddFilterableStringAndContentString("SeoInfo_MetaKeywords", seoInfo.MetaKeywords);
                    }
                    if (!string.IsNullOrEmpty(seoInfo.SemanticUrl))
                    {
                        document.AddFilterableStringAndContentString("SeoInfo_Slug", seoInfo.SemanticUrl);
                    }
                }
            }

            documents.Add(document);
        }

        return documents;
    }
}

