using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.News.Core;
using VirtoCommerce.News.Core.Models;
using VirtoCommerce.News.Core.Services;
using VirtoCommerce.Platform.Core.Common;

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
    [Route("")]
    [Authorize(ModuleConstants.Security.Permissions.Create)]
    [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
    public async Task<ActionResult> Create([FromBody] NewsArticle newsArticle)
    {
        newsArticle.Id = null;
        await _newsArticleService.SaveChangesAsync([newsArticle]);
        return NoContent();
    }

    [HttpPut]
    [Route("")]
    [Authorize(ModuleConstants.Security.Permissions.Update)]
    [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
    public async Task<ActionResult> Update([FromBody] NewsArticle newsArticle)
    {
        await _newsArticleService.SaveChangesAsync([newsArticle]);
        return NoContent();
    }

    [HttpDelete]
    [Route("")]
    [Authorize(ModuleConstants.Security.Permissions.Delete)]
    [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
    public async Task<ActionResult> Delete([FromQuery] string[] ids)
    {
        await _newsArticleService.DeleteAsync(ids);
        return NoContent();
    }

    [HttpGet]
    [Route("{id}")]
    [Authorize(ModuleConstants.Security.Permissions.Read)]
    public async Task<ActionResult<NewsArticle>> Get([FromRoute] string id)
    {
        var result = await _newsArticleService.GetByIdAsync(id);
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
