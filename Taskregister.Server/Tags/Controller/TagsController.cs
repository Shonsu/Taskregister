using Microsoft.AspNetCore.Mvc;
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
        var readOnlyList = await tagsService.ListTags();
        return Ok(readOnlyList);
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateTag([FromBody] TagDto tagDto)
    {
        var tagId = await tagsService.CreateTag(new Tag { Name = tagDto.Name });
        return Ok(tagId);
    }

    [HttpDelete("{tagId}")]
    public async Task<IActionResult> CreateTag([FromRoute] int tagId)
    {
        var deletedTagId = await tagsService.DeleteTag(tagId);
        return Ok(deletedTagId);
    }

    [HttpPut("{tagId}")]
    public async Task<IActionResult> UpdateTag([FromBody] TagDto tagDto, [FromRoute] int tagId)
    {
        var updateTagId = await tagsService.UpdateTag(tagId, new Tag { Name = tagDto.Name });
        return Ok(updateTagId);
    }
}