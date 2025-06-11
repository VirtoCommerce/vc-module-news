using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.News.Core;
using VirtoCommerce.News.Core.Models;
using VirtoCommerce.News.Core.Services;

namespace VirtoCommerce.News.Web.Controllers.Api;

[Authorize]
[Route("api/news")]
public class NewsArticleController : Controller
{
    private readonly INewsArticleService _newsArticleService;
    private readonly INewsArticleSearchService _newsArticleSearchService;

    public NewsArticleController(INewsArticleService newsArticleService, INewsArticleSearchService newsArticleSearchService)
    {
        _newsArticleService = newsArticleService;
        _newsArticleSearchService = newsArticleSearchService;
    }

    [HttpPost]
    [Route("save")]
    [Authorize(ModuleConstants.Security.Permissions.Create)]
    public async Task<ActionResult<NewsArticle>> Save([FromBody] NewsArticle newsArticle)
    {
        await _newsArticleService.SaveChangesAsync([newsArticle]);

        return NoContent();
    }

    [HttpDelete]
    [Route("delete")]
    [Authorize(ModuleConstants.Security.Permissions.Delete)]
    public async Task<ActionResult<NewsArticle>> Delete([FromQuery] string[] ids)
    {
        await _newsArticleService.DeleteAsync(ids);

        return NoContent();
    }

    [HttpGet]
    [Route("get")]
    [Authorize(ModuleConstants.Security.Permissions.Read)]
    public async Task<ActionResult<NewsArticleSearchResult>> Get([FromQuery] string[] ids)
    {
        var result = await _newsArticleService.GetAsync(ids);

        return Ok(result);
    }

    [HttpGet]
    [Route("get-all")]
    [Authorize(ModuleConstants.Security.Permissions.Read)]
    public async Task<ActionResult<NewsArticleSearchResult>> GetAll()
    {
        var result = await _newsArticleSearchService.SearchAsync(new NewsArticleSearchCriteria());

        return Ok(result);
    }
}
