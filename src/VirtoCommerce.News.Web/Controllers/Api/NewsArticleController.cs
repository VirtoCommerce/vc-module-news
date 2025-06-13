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
    [Route("create")]
    [Authorize(ModuleConstants.Security.Permissions.Create)]
    public async Task<ActionResult<NewsArticle>> Create([FromBody] NewsArticle newsArticle)
    {
        await _newsArticleService.SaveChangesAsync([newsArticle]);

        return Ok(newsArticle);
    }

    [HttpPost]
    [Route("update")]
    [Authorize(ModuleConstants.Security.Permissions.Update)]
    public async Task<ActionResult<NewsArticle>> Update([FromBody] NewsArticle newsArticle)
    {
        await _newsArticleService.SaveChangesAsync([newsArticle]);

        return Ok(newsArticle);
    }

    [HttpDelete]
    [Route("delete")]
    [Authorize(ModuleConstants.Security.Permissions.Delete)]
    public async Task<ActionResult> Delete([FromQuery] string[] ids)
    {
        await _newsArticleService.DeleteAsync(ids);

        return NoContent();
    }

    [HttpGet]
    [Route("get")]
    [Authorize(ModuleConstants.Security.Permissions.Read)]
    public async Task<ActionResult<NewsArticle[]>> Get([FromQuery] string[] ids)
    {
        var result = await _newsArticleService.GetAsync(ids);

        return Ok(result);
    }

    [HttpPost]
    [Route("search")]
    [Authorize(ModuleConstants.Security.Permissions.Read)]
    public async Task<ActionResult<NewsArticleSearchResult>> Search([FromBody] NewsArticleSearchCriteria criteria)
    {
        var result = await _newsArticleSearchService.SearchAsync(criteria, false);

        return Ok(result);
    }
}
