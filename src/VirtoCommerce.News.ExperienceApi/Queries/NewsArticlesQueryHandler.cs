using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.CustomerModule.Core.Services;
using VirtoCommerce.News.Core.Models;
using VirtoCommerce.News.Core.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.Seo.Core.Extensions;
using VirtoCommerce.Seo.Core.Models;
using VirtoCommerce.StoreModule.Core.Services;
using VirtoCommerce.Xapi.Core;
using VirtoCommerce.Xapi.Core.Infrastructure;
using NewsSettings = VirtoCommerce.News.Core.ModuleConstants.Settings.General;

namespace VirtoCommerce.News.ExperienceApi.Queries;

public class NewsArticlesQueryHandler(
    INewsArticleService newsArticleService,
    INewsArticleSearchService newsArticleSearchService,
    IMemberResolver memberResolver,
    IMemberService memberService,
    IStoreService storeService,
    INewsArticleSeoResolver newsArticleSeoResolver,
    ISettingsManager settingsManager)
    : IQueryHandler<NewsArticleQuery, NewsArticle>,
      IQueryHandler<NewsArticlesQuery, NewsArticleSearchResult>
{
    public async Task<NewsArticle> Handle(NewsArticleQuery request, CancellationToken cancellationToken)
    {
        var result = await newsArticleService.GetNoCloneAsync(request.Id);

        if (result == null)
        {
            var useRootLinks = await settingsManager.GetValueAsync<bool>(NewsSettings.UseRootLinks);

            if (!useRootLinks)
            {
                var newsArticleSeoInfo = await newsArticleSeoResolver.FindActiveSeoAsync([request.Id], request.StoreId, request.LanguageCode);

                if (!newsArticleSeoInfo.IsNullOrEmpty())
                {
                    result = await newsArticleService.GetNoCloneAsync(newsArticleSeoInfo.First().ObjectId);
                }
            }
        }

        if (result != null)
        {
            await PostProcessResultAsync(request.StoreId, request.LanguageCode, [result]);
        }

        return result;
    }

    public async Task<NewsArticleSearchResult> Handle(NewsArticlesQuery request, CancellationToken cancellationToken)
    {
        var searchCriteria = await BuildSearchCriteria(request);

        var result = await newsArticleSearchService.SearchAsync(searchCriteria);

        if (!result.Results.IsNullOrEmpty())
        {
            await PostProcessResultAsync(request.StoreId, request.LanguageCode, result.Results);
        }

        return result;
    }

    protected virtual async Task<NewsArticleSearchCriteria> BuildSearchCriteria(NewsArticlesQuery request)
    {
        var userGroups = await GetUserGroups(request.UserId);

        return new NewsArticleSearchCriteria
        {
            LanguageCode = request.LanguageCode,
            Keyword = request.Keyword,
            StoreId = request.StoreId,
            UserGroups = userGroups,
            Published = true,
            Sort = nameof(NewsArticle.PublishDate),
            Skip = request.Skip,
            Take = request.Take,
        };
    }

    protected virtual async Task PostProcessResultAsync(string storeId, string languageCode, IList<NewsArticle> newsArticles)
    {
        var store = !storeId.IsNullOrEmpty() ? await storeService.GetNoCloneAsync(storeId) : null;

        await FilterLanguagesAsync(newsArticles, languageCode, store?.DefaultLanguage);
        await FilterSeoInfosAsync(newsArticles, languageCode, storeId, store?.DefaultLanguage);
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

    protected virtual Task FilterLanguagesAsync(IList<NewsArticle> newsArticles, string languageCode, string storeDefaultLanguage)
    {
        foreach (var newsArticle in newsArticles)
        {
            var allLocalizedContents = newsArticle.LocalizedContents;

            newsArticle.LocalizedContents = allLocalizedContents
                .Where(x => x.LanguageCode.EqualsIgnoreCase(languageCode))
                .ToList();

            if (newsArticle.LocalizedContents.Count == 0)
            {
                newsArticle.LocalizedContents = allLocalizedContents
                    .Where(x => x.LanguageCode.EqualsIgnoreCase(storeDefaultLanguage))
                    .ToList();
            }
        }

        return Task.CompletedTask;
    }

    protected virtual Task FilterSeoInfosAsync(IList<NewsArticle> newsArticles, string languageCode, string storeId, string storeDefaultLanguage)
    {
        foreach (var newsArticle in newsArticles)
        {
            SeoInfo seoInfo = null;
            var activeSeoInfos = newsArticle.SeoInfos?.Where(x => x.IsActive).ToList();

            if (!activeSeoInfos.IsNullOrEmpty())
            {
                seoInfo = activeSeoInfos.GetBestMatchingSeoInfo(storeId, storeDefaultLanguage, languageCode);
            }

            if (seoInfo == null)
            {
                seoInfo = SeoExtensions.GetFallbackSeoInfo(newsArticle.Id, newsArticle.Name, languageCode);
            }

            newsArticle.SeoInfos.Clear();
            newsArticle.SeoInfos.Add(seoInfo);
        }

        return Task.CompletedTask;
    }
}
