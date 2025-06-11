using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Permissions = VirtoCommerce.News.Core.ModuleConstants.Security.Permissions;

namespace VirtoCommerce.News.Web.Controllers.Api;

[Authorize]
[Route("api/news")]
public class NewsController : Controller
{
    // GET: api/news
    /// <summary>
    /// Get message
    /// </summary>
    /// <remarks>Return "Hello world!" message</remarks>
    [HttpGet]
    [Route("")]
    [Authorize(Permissions.Read)]
    public ActionResult<string> Get()
    {
        return Ok(new { result = "Hello world!" });
    }
}
