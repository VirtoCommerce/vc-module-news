using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.CustomerModule.Core.Services;
using VirtoCommerce.News.Core.Models;
using VirtoCommerce.News.Core.Services;
using VirtoCommerce.News.ExperienceApi.Model;
using VirtoCommerce.News.ExperienceApi.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Seo.Core.Extensions;
using VirtoCommerce.Seo.Core.Models;
using VirtoCommerce.Xapi.Core;
using VirtoCommerce.Xapi.Core.Infrastructure;

namespace VirtoCommerce.News.ExperienceApi.Queries;

public class NewsArticlesQueryHandler(
    INewsArticleService newsArticleService,
    INewsArticleSearchService newsArticleSearchService,
    IMemberResolver memberResolver,
    IMemberService memberService,
    INewsArticleSeoService newsArticleSeoService,
    INewsArticleSettingsService newsArticleSettingsService)
    : IQueryHandler<NewsArticleQuery, NewsArticle>,
      IQueryHandler<NewsArticlesQuery, NewsArticleSearchResult>
{
    public async Task<NewsArticle> Handle(NewsArticleQuery request, CancellationToken cancellationToken)
    {
        var result = await newsArticleService.GetNoCloneAsync(request.Id);

        if (result == null)
        {
            var settings = await newsArticleSettingsService.GetSettingsAsync(request.StoreId);

            if (settings.UseNewsPrefixInLinks)
            {
                var newsArticleSeoInfo = await newsArticleSeoService.FindActiveSeoAsync([request.Id], request.StoreId, request.LanguageCode);

                if (!newsArticleSeoInfo.IsNullOrEmpty())
                {
                    result = await newsArticleService.GetNoCloneAsync(newsArticleSeoInfo.First().ObjectId);
                }
            }
        }

        if (result != null)
        {
            var settings = await newsArticleSettingsService.GetSettingsAsync(request.StoreId);
            PostProcessResult([result], request.StoreId, request.LanguageCode, settings);
        }

        return result;
    }

    public async Task<NewsArticleSearchResult> Handle(NewsArticlesQuery request, CancellationToken cancellationToken)
    {
        var settings = await newsArticleSettingsService.GetSettingsAsync(request.StoreId);
        var searchCriteria = await BuildSearchCriteria(request, settings);
        var result = await newsArticleSearchService.SearchAsync(searchCriteria);

        if (!result.Results.IsNullOrEmpty())
        {
            PostProcessResult(result.Results, request.StoreId, request.LanguageCode, settings);
        }

        return result;
    }

    protected virtual async Task<NewsArticleSearchCriteria> BuildSearchCriteria(NewsArticlesQuery request, NewsArticleSettings settings)
    {
        var languageCodes = new List<string>() { request.LanguageCode };

        if (settings.UseStoreDefaultLanguage && !settings.StoreDefaultLanguage.IsNullOrEmpty())
        {
            languageCodes.Add(settings.StoreDefaultLanguage);
        }

        var userGroups = await GetUserGroups(request.UserId);

        var result = AbstractTypeFactory<NewsArticleSearchCriteria>.TryCreateInstance();

        result.LanguageCodes = languageCodes;
        result.Keyword = request.Keyword;
        result.StoreId = request.StoreId;
        result.UserGroups = userGroups;
        result.Published = true;
        result.Sort = nameof(NewsArticle.PublishDate);
        result.Skip = request.Skip;
        result.Take = request.Take;

        return result;
    }

    protected virtual void PostProcessResult(IList<NewsArticle> newsArticles, string requestStoreId, string requestLanguageCode, NewsArticleSettings settings)
    {
        FilterLanguages(newsArticles, requestLanguageCode, settings);
        FilterSeoInfos(newsArticles, requestStoreId, requestLanguageCode, settings);
    }

    protected virtual async Task<IList<string>> GetUserGroups(string userId)
    {
        var result = new List<string>();

        if (!string.IsNullOrEmpty(userId) && !ModuleConstants.AnonymousUser.UserName.EqualsIgnoreCase(userId))
        {
            var member = await memberResolver.ResolveMemberByIdAsync(userId);

            if (member is Contact contact)
            {
                result.AddRange(await GetUserGroupsInheritedAsync(contact));
            }
        }

        return result;
    }

    protected async Task<IList<string>> GetUserGroupsInheritedAsync(Contact contact)
    {
        var userGroups = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        if (!contact.Groups.IsNullOrEmpty())
        {
            userGroups.AddRange(contact.Groups);
        }

        if (!contact.Organizations.IsNullOrEmpty())
        {
            var organizations = await memberService.GetByIdsAsync(contact.Organizations.ToArray(), nameof(MemberResponseGroup.WithGroups));
            userGroups.AddRange(organizations.OfType<Organization>().SelectMany(x => x.Groups));
        }

        return userGroups.ToList();
    }

    protected virtual void FilterLanguages(IList<NewsArticle> newsArticles, string requestLanguageCode, NewsArticleSettings settings)
    {
        foreach (var newsArticle in newsArticles)
        {
            newsArticle.LocalizedContents = GetBestMatchingContent(newsArticle.LocalizedContents, requestLanguageCode, settings.UseStoreDefaultLanguage ? settings.StoreDefaultLanguage : null);
        }
    }

    protected virtual void FilterSeoInfos(IList<NewsArticle> newsArticles, string requestStoreId, string requestLanguageCode, NewsArticleSettings settings)
    {
        foreach (var newsArticle in newsArticles)
        {
            SeoInfo seoInfo = null;
            var activeSeoInfos = newsArticle.SeoInfos?.Where(x => x.IsActive).ToList();

            var localizedContents = GetBestMatchingContent(newsArticle.LocalizedContents, requestLanguageCode, settings.UseStoreDefaultLanguage ? settings.StoreDefaultLanguage : null);

            if (!activeSeoInfos.IsNullOrEmpty())
            {
                seoInfo = activeSeoInfos.GetBestMatchingSeoInfo(requestStoreId, settings.StoreDefaultLanguage, localizedContents.FirstOrDefault()?.LanguageCode);
                if (seoInfo != null && seoInfo.Name.IsNullOrEmpty())
                {
                    seoInfo.Name = newsArticle.Name;
                }
            }

            if (seoInfo == null)
            {
                seoInfo = SeoExtensions.GetFallbackSeoInfo(newsArticle.Id, newsArticle.Name, localizedContents.FirstOrDefault()?.LanguageCode);
            }

            newsArticle.SeoInfos.Clear();
            newsArticle.SeoInfos.Add(seoInfo);
        }
    }

    protected virtual IList<NewsArticleLocalizedContent> GetBestMatchingContent(IList<NewsArticleLocalizedContent> localizedContents, string languageCode, string storeDefaultLanguage)
    {
        var result = localizedContents
            .Where(x => x.LanguageCode.EqualsIgnoreCase(languageCode))
            .ToList();

        if ((result.Count == 0) && !storeDefaultLanguage.IsNullOrEmpty())
        {
            result = localizedContents
                .Where(x => x.LanguageCode.EqualsIgnoreCase(storeDefaultLanguage))
                .ToList();
        }

        return result;
    }
}
