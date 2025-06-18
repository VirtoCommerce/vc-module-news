using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using VirtoCommerce.Platform.Core;

namespace VirtoCommerce.News.ExperienceApi.Authorization;

public class NewsArticleContentAuthorizationHandler : AuthorizationHandler<NewsArticleContentAuthorizationRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, NewsArticleContentAuthorizationRequirement requirement)
    {
        var isAuthorized = context.User.IsInRole(PlatformConstants.Security.SystemRoles.Administrator);
        if (isAuthorized)
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        var isAuthenticated = context.User.Identity?.IsAuthenticated ?? false;
        if (isAuthenticated)
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        context.Fail();
        return Task.CompletedTask;
    }
}
