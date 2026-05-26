using Microsoft.AspNetCore.Mvc;
using BLL.DTOs.Category;
using BLL.Interfaces;

namespace PL.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpPost]
    public async Task<ActionResult<CategoryResponse>> Create([FromBody] CreateCategoryRequest request, CancellationToken cancellationToken)
    {
        var id = await _categoryService.CreateAsync(request, cancellationToken);
        var categoryResponse = await _categoryService.GetByIdAsync(id, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id }, categoryResponse);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _categoryService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpPatch]
    public async Task<IActionResult> Update([FromBody] UpdateCategoryRequest request, CancellationToken cancellationToken)
    {
        await _categoryService.UpdateAsync(request, cancellationToken);
        return NoContent();
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CategoryResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var category = await _categoryService.GetByIdAsync(id, cancellationToken);
        return Ok(category);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var categories = await _categoryService.GetAllAsync(cancellationToken);
        return Ok(categories);
    }

    [HttpGet("{id:guid}/subcategories")]
    public async Task<ActionResult<IEnumerable<CategoryResponse>>> GetSubcategories(Guid id, CancellationToken cancellationToken)
    {
        var subcategories = await _categoryService.GetSubcategoriesAsync(id, cancellationToken);
        return Ok(subcategories);
    }
}