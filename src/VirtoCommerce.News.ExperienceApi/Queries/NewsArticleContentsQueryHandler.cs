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
using VirtoCommerce.Seo.Core.Extensions;
using VirtoCommerce.Seo.Core.Models;
using VirtoCommerce.StoreModule.Core.Model;
using VirtoCommerce.StoreModule.Core.Services;
using VirtoCommerce.Xapi.Core;
using VirtoCommerce.Xapi.Core.Infrastructure;

namespace VirtoCommerce.News.ExperienceApi.Queries;

public class NewsArticleContentsQueryHandler(
    INewsArticleService newsArticleService,
    INewsArticleSearchService newsArticleSearchService,
    IMemberResolver memberResolver,
    IMemberService memberService,
    IStoreService storeService)
    : IQueryHandler<NewsArticleContentsQuery, NewsArticleSearchResult>,
    IQueryHandler<NewsArticleContentQuery, NewsArticle>
{
    public async Task<NewsArticleSearchResult> Handle(NewsArticleContentsQuery request, CancellationToken cancellationToken)
    {
        var searchCriteria = await BuildSearchCriteria(request);

        var result = await newsArticleSearchService.SearchAsync(searchCriteria);

        await PostProcessResultAsync(request.StoreId, request.LanguageCode, result.Results);

        return result;
    }

    public async Task<NewsArticle> Handle(NewsArticleContentQuery request, CancellationToken cancellationToken)
    {
        var result = await newsArticleService.GetByIdAsync(request.Id);

        await PostProcessResultAsync(request.StoreId, request.LanguageCode, [result]);

        return result;
    }

    protected virtual async Task<NewsArticleSearchCriteria> BuildSearchCriteria(NewsArticleContentsQuery request)
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
        Store store = !storeId.IsNullOrEmpty() ? await storeService.GetNoCloneAsync(storeId) : null;

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

            if (!newsArticle.SeoInfos.IsNullOrEmpty())
            {
                seoInfo = newsArticle.SeoInfos.GetBestMatchingSeoInfo(storeId, storeDefaultLanguage, languageCode);
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
