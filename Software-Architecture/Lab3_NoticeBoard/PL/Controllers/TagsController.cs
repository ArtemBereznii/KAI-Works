using Microsoft.AspNetCore.Mvc;
using BLL.DTOs.Tag;
using BLL.Interfaces;

namespace PL.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TagsController : ControllerBase
{
    private readonly ITagService _tagService;

    public TagsController(ITagService tagService)
    {
        _tagService = tagService;
    }

    [HttpPost]
    public async Task<ActionResult<TagResponse>> Create([FromBody] CreateTagRequest request, CancellationToken cancellationToken)
    {
        var id = await _tagService.CreateAsync(request, cancellationToken);
        var tagResponse = await _tagService.GetByIdAsync(id, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id }, tagResponse);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _tagService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpPatch]
    public async Task<IActionResult> Update([FromBody] UpdateTagRequest request, CancellationToken cancellationToken)
    {
        await _tagService.UpdateAsync(request, cancellationToken);
        return NoContent();
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TagResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var tag = await _tagService.GetByIdAsync(id, cancellationToken);
        return Ok(tag);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TagResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var tags = await _tagService.GetAllAsync(cancellationToken);
        return Ok(tags);
    }
}