using System.Security.Claims;
using main.Data.Model.Account;
using main.Database.Model.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace main.Controllers.Account;

[ApiController]
[Route("account")]
public class AccountController : ControllerBase
{

    private readonly UserManager<MainUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AccountController(UserManager<MainUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }
    
    [AllowAnonymous]
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] MainUserModel mainUserModel)
    {
        if (await _userManager.FindByEmailAsync(mainUserModel.Email) != null)
        {
            return BadRequest();
        }
        
        var mainUser = new MainUser
        {
            Email = mainUserModel.Email,
            UserName = mainUserModel.Name
        };

        var result = await _userManager.CreateAsync(mainUser, mainUserModel.Password);
        
        if (result.Errors.Any())
        {
            return BadRequest(result.Errors);
        }

        await _userManager.AddToRoleAsync(mainUser, "User");
        await _userManager.AddClaimsAsync(mainUser, new[]
        {
            new Claim(ClaimTypes.Email, mainUser.Email),
            new Claim(ClaimTypes.Name, mainUser.UserName)
        });

        return Ok();
    }

    [Authorize(Policy = "RequireAdmin")]
    [HttpPost("bind-role")]
    public async Task<IActionResult> BindRole([FromBody] BindRoleModel bindRoleModel)
    {
        var user = await _userManager.FindByEmailAsync(bindRoleModel.Email);
        var role = await _roleManager.FindByNameAsync(bindRoleModel.Role);
        
        if (user == null || role == null)
        {
            return NotFound();
        }

        await _userManager.AddToRoleAsync(user, role.Name!);

        return Ok();
    }

    [Authorize(Policy = "HasClaimEmailAddress")]
    [HttpGet("read")]
    public async Task<IActionResult> Read()
    {
        var user = await _userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Email) ?? "");

        if (user == null)
        {
            return NotFound();
        }

        var mainUserModel = new MainUserModel
        {
            Email = user.Email!,
            Name = user.UserName!,
            Password = user.PasswordHash!
        };

        return Ok(mainUserModel);
    }

    [Authorize(Policy = "RequireAdminOrUser")]
    [Authorize(Policy = "HasClaimEmailAddress")]
    [HttpPut("update")]
    public async Task<IActionResult> UpdateName([FromBody] string name)
    {
        var email = User.FindFirstValue(ClaimTypes.Email) ?? "";
        var user = await _userManager.FindByEmailAsync(email);
        
        if (user == null)
        {
            return BadRequest();
        }

        if (user.UserName == name)
        {
            return NoContent();
        }

        user.UserName = name;
        
        var result = await _userManager.UpdateAsync(user);

        if (result.Errors.Any())
        {
            return BadRequest(result.Errors);
        }

        return Ok();
    }
}