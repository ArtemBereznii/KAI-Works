using BLL.DTOs.Advertisement;
using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;

namespace PL.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdvertisementsController : ControllerBase
{
    private readonly IAdvertisementService _advertisementService;

    public AdvertisementsController(IAdvertisementService advertisementService)
    {
        _advertisementService = advertisementService;
    }

    [HttpPost]
    public async Task<ActionResult<AdvertisementResponse>> Create([FromBody] CreateAdvertisementRequest request, CancellationToken cancellationToken)
    {
        var id = await _advertisementService.CreateAsync(request, cancellationToken);
        var advertisementResponse = await _advertisementService.GetByIdAsync(id, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id }, advertisementResponse);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] DeleteAdvertisementRequest request, CancellationToken cancellationToken)
    {
        await _advertisementService.DeleteAsync(request, cancellationToken);
        return NoContent();
    }

    [HttpPatch]
    public async Task<IActionResult> Update([FromBody] UpdateAdvertisementRequest request, CancellationToken cancellationToken)
    {
        await _advertisementService.UpdateAsync(request, cancellationToken);
        return NoContent();
    }

    [HttpPatch("deactivate")]
    public async Task<IActionResult> Deactivate([FromBody] DeactivateAdvertisementRequest request, CancellationToken cancellationToken)
    {
        await _advertisementService.DeactivateAsync(request, cancellationToken);
        return NoContent();
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AdvertisementResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var advertisement = await _advertisementService.GetByIdAsync(id, cancellationToken);
        return Ok(advertisement);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AdvertisementResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var advertisements = await _advertisementService.GetAllAsync(cancellationToken);
        return Ok(advertisements);
    }

    [HttpGet("active")]
    public async Task<ActionResult<IEnumerable<AdvertisementResponse>>> GetActive(CancellationToken cancellationToken)
    {
        var advertisements = await _advertisementService.GetActiveAsync(cancellationToken);
        return Ok(advertisements);
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<AdvertisementResponse>>> Search([FromQuery] SearchAdvertisementsRequest request, CancellationToken cancellationToken)
    {
        var advertisements = await _advertisementService.SearchAsync(request, cancellationToken);
        return Ok(advertisements);
    }
}