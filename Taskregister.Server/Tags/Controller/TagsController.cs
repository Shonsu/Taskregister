using Microsoft.AspNetCore.Mvc;
using Taskregister.Server.Shared;
using Taskregister.Server.Tags.Controller.Dtos;
using Taskregister.Server.Tags.Entities;
using Taskregister.Server.Tags.Services;

namespace Taskregister.Server.Tags.Controller;

[ApiController]
[Route("api/[controller]")]
public class TagsController(ITagsService tagsService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TagDto>>> GetAllAsync()
    {
        var tags = await tagsService.ListTags();
        return Ok(tags);
    }

    [HttpGet("{tagId}")]
    public async Task<ActionResult<Tag>> GetById([FromRoute] int tagId)
    {
        var tag = await tagsService.GetTagById(tagId);
        return tag.Match(onSuccess: Ok, onFailure: NotFound);
    }

    [HttpPost]
    public async Task<IActionResult> CreateTag([FromBody] TagDto tagDto)
    {
        var result = await tagsService.CreateTag(new Tag { Name = tagDto.Name });
        return result.Match(onSuccess: r => Ok(r), onFailure: e => BadRequest(e));
    }

    [HttpDelete("{tagId}")]
    public async Task<ActionResult<Result<int>>> DeleteTag([FromRoute] int tagId)
    {
        var result = await tagsService.DeleteTag(tagId);
        return result.Match(onSuccess: r => Ok(r), onFailure: NotFound);
    }

    [HttpPut("{tagId}")]
    public async Task<ActionResult<Result<int>>> UpdateTag([FromBody] TagDto tagDto, [FromRoute] int tagId)
    {
        var result = await tagsService.UpdateTag(tagId, new Tag { Name = tagDto.Name });
        return result.Match(onSuccess: r => Ok(r), onFailure: NotFound);
    }
}