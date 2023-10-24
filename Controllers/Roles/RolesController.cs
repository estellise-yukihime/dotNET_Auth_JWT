using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace main.Controllers.Roles;

[Authorize(Policy = "RequireAdmin")]
[ApiController]
[Route("roles")]
public class RolesController : ControllerBase
{
    private readonly RoleManager<IdentityRole> _roleManager;

    public RolesController(RoleManager<IdentityRole> roleManager)
    {
        _roleManager = roleManager;
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] string role)
    {
        if (await _roleManager.RoleExistsAsync(role))
        {
            return Conflict();
        }

        await _roleManager.CreateAsync(new IdentityRole(role));

        return Created("", null);
    }
}