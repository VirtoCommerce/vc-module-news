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
public class NewsArticleController(INewsArticleService newsArticleService, INewsArticleSearchService newsArticleSearchService) : Controller
{
    [HttpPost]
    [Route("")]
    [Authorize(ModuleConstants.Security.Permissions.Create)]
    public async Task<ActionResult<NewsArticle>> Create([FromBody] NewsArticle newsArticle)
    {
        newsArticle.Id = null;
        newsArticle.LocalizedContents = null;
        newsArticle.SeoInfos = null;

        await newsArticleService.SaveChangesAsync([newsArticle]);
        return Ok(newsArticle);
    }

    [HttpPost]
    [Route("clone")]
    [Authorize(ModuleConstants.Security.Permissions.Create)]
    public async Task<ActionResult<NewsArticle>> Clone([FromBody] NewsArticle newsArticle)
    {
        var clonedNewsArticle = await newsArticleService.Clone(newsArticle);
        return Ok(clonedNewsArticle);
    }

    [HttpPut]
    [Route("")]
    [Authorize(ModuleConstants.Security.Permissions.Update)]
    public async Task<ActionResult<NewsArticle>> Update([FromBody] NewsArticle newsArticle)
    {
        await newsArticleService.SaveChangesAsync([newsArticle]);
        return Ok(newsArticle);
    }

    [HttpDelete]
    [Route("")]
    [Authorize(ModuleConstants.Security.Permissions.Delete)]
    [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
    public async Task<ActionResult> Delete([FromQuery] string[] ids)
    {
        await newsArticleService.DeleteAsync(ids);
        return NoContent();
    }

    [HttpGet]
    [Route("{id}")]
    [Authorize(ModuleConstants.Security.Permissions.Read)]
    public async Task<ActionResult<NewsArticle>> Get([FromRoute] string id)
    {
        var result = await newsArticleService.GetNoCloneAsync(id);
        return Ok(result);
    }

    [HttpPost]
    [Route("search")]
    [Authorize(ModuleConstants.Security.Permissions.Read)]
    public async Task<ActionResult<NewsArticleSearchResult>> Search([FromBody] NewsArticleSearchCriteria criteria)
    {
        var result = await newsArticleSearchService.SearchNoCloneAsync(criteria);
        return Ok(result);
    }

    [HttpPost]
    [Route("publish")]
    [Authorize(ModuleConstants.Security.Permissions.Publish)]
    [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
    public async Task<ActionResult> Publish([FromBody] string[] ids)
    {
        await newsArticleService.PublishAsync(ids);
        return NoContent();
    }

    [HttpPost]
    [Route("unpublish")]
    [Authorize(ModuleConstants.Security.Permissions.Publish)]
    [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
    public async Task<ActionResult> Unpublish([FromBody] string[] ids)
    {
        await newsArticleService.UnpublishAsync(ids);
        return NoContent();
    }

    [HttpPost]
    [Route("archive")]
    [Authorize(ModuleConstants.Security.Permissions.Publish)]
    [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
    public async Task<ActionResult> Archive([FromBody] string[] ids)
    {
        await newsArticleService.ArchiveAsync(ids);
        return NoContent();
    }

    [HttpPost]
    [Route("unarchive")]
    [Authorize(ModuleConstants.Security.Permissions.Publish)]
    [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
    public async Task<ActionResult> Unarchive([FromBody] string[] ids)
    {
        await newsArticleService.UnarchiveAsync(ids);
        return NoContent();
    }
}
