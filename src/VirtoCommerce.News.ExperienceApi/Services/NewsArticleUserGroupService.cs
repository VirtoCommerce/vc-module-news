using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.CustomerModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Xapi.Core;

namespace VirtoCommerce.News.ExperienceApi.Services;

public class NewsArticleUserGroupService(IMemberResolver memberResolver, IMemberService memberService) : INewsArticleUserGroupService
{
    public virtual async Task<IList<string>> GetUserGroups(string userId)
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

    protected virtual async Task<IList<string>> GetUserGroupsInheritedAsync(Contact contact)
    {
        var userGroups = new List<string>();

        if (!contact.Groups.IsNullOrEmpty())
        {
            userGroups.AddRange(contact.Groups);
        }

        if (!contact.Organizations.IsNullOrEmpty())
        {
            var organizations = await memberService.GetByIdsAsync(contact.Organizations.ToArray(), MemberResponseGroup.WithGroups.ToString());
            userGroups.AddRange(organizations.OfType<Organization>().SelectMany(x => x.Groups));
        }

        return userGroups;
    }
}
