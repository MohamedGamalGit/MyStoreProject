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

        if (!Guid.TryParse(userIdClaim, out var userId)) return Unauthorized();

        var permissions = await _svc.GetUserPermissionsAsync(userId);

        // optional: include pages/actions structured
        var grouped = permissions
            .Select(p => {
                var parts = p.Split('.');
                return new { Page = parts[0], Action = parts.Length > 1 ? parts[1] : "" };
            })
            .GroupBy(x => x.Page)
            .ToDictionary(g => g.Key, g => g.Select(x => x.Action).ToList());

        return Ok(new
        {
            userId = userId,
            permissions = permissions,
            structured = grouped
        });
    }
}