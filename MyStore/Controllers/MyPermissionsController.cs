using Microsoft.AspNetCore.Mvc;
using Models.Models;
using Services.Interfaces;

[Route("api/[controller]")]
[ApiController]
public class MyPermissionsController : ControllerBase
{
    private readonly IPermissionService _svc;
    public MyPermissionsController(IPermissionService svc) { _svc = svc; }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyPermissions()
    {
        var userIdClaim = User.FindFirst("UserId")?.Value;
        if (userIdClaim == null) return Unauthorized();

        if (!Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var roles = await _svc.GetRolesWithPermissionsAsync();

        return Ok(new
        {
            userId = userId,
            roles = roles
        });
    }


}