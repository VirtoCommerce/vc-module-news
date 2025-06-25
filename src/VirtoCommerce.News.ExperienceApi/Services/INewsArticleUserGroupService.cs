using System.Collections.Generic;
using System.Threading.Tasks;

namespace VirtoCommerce.News.ExperienceApi.Services;

public interface INewsArticleUserGroupService
{
    Task<IList<string>> GetUserGroups(string userId);
}
