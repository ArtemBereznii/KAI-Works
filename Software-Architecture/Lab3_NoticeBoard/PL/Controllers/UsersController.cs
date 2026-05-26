using Microsoft.AspNetCore.Mvc;
using BLL.DTOs.User;
using BLL.Interfaces;

namespace PL.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost]
    public async Task<ActionResult<UserResponse>> Create([FromBody] CreateUserRequest request, CancellationToken cancellationToken)
    {
        var id = await _userService.CreateAsync(request, cancellationToken);
        var userResponse = await _userService.GetByIdAsync(id, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id }, userResponse);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _userService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpPatch]
    public async Task<IActionResult> Update([FromBody] UpdateUserRequest request, CancellationToken cancellationToken)
    {
        await _userService.UpdateAsync(request, cancellationToken);
        return NoContent();
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var user = await _userService.GetByIdAsync(id, cancellationToken);
        return Ok(user);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var users = await _userService.GetAllAsync(cancellationToken);
        return Ok(users);
    }
}