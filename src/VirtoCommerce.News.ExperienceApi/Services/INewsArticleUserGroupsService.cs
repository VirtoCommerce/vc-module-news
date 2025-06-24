using System.Collections.Generic;
using System.Threading.Tasks;

namespace VirtoCommerce.News.ExperienceApi.Services;

public interface INewsArticleUserGroupsService
{
    Task<IList<string>> GetUserGroups(string userId);
}
