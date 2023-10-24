using System.IdentityModel.Tokens.Jwt;
using main.Data.Model.Token;
using main.Database.Model.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace main.Controllers.Token;

[ApiController]
[Route("token")]
public class TokenController : ControllerBase
{
    private readonly UserManager<MainUser> _userManager;

    public TokenController(UserManager<MainUser> userManager)
    {
        _userManager = userManager;
    }

    [HttpPost("generate")]
    public async Task<IActionResult> Generate([FromBody] TokenGenerateModel tokenGenerateModel)
    {
        var user = await _userManager.FindByEmailAsync(tokenGenerateModel.Email);

        if (user == null)
        {
            return NotFound();
        }

        var result = _userManager.PasswordHasher.VerifyHashedPassword(user, user.PasswordHash!, tokenGenerateModel.Password);

        if (result != PasswordVerificationResult.Success)
        {
            return BadRequest();
        }

        var roles = await _userManager.GetRolesAsync(user);
        var claims = await _userManager.GetClaimsAsync(user);
        
        // var a = new JwtSecurityToken()
        
        // generate token

        return Ok("");
    }
}