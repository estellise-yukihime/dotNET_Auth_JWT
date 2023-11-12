using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using main.Data.Model.Token;
using main.Database.Storage.MainUser.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace main.Controllers.Token;

[ApiController]
[Route("token")]
public class TokenController : ControllerBase
{
    private readonly UserManager<MainUser> _userManager;
    private readonly IConfiguration _configuration;
    
    public TokenController(UserManager<MainUser> userManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
    }

    [AllowAnonymous]
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

        var claims = await GenerateClaims(user);
        var token = GenerateToken(claims);

        if (token == null)
        {
            return StatusCode(500);
        }

        return Ok(token);
    }

    private async Task<IEnumerable<Claim>> GenerateClaims(MainUser mainUser)
    {
        var roles = await _userManager.GetRolesAsync(mainUser);
        var claims = await _userManager.GetClaimsAsync(mainUser);

        var rolesClaims = roles.Select(x => new Claim(ClaimTypes.Role, x)).ToList();
        var totalClaims = new List<Claim>();
        
        totalClaims.AddRange(claims);
        totalClaims.AddRange(rolesClaims);

        return totalClaims;
    }

    private string? GenerateToken(IEnumerable<Claim> claims)
    {
        var jwtSection = _configuration.GetSection("JWT");
        var jwtKey = jwtSection["Key"];
        var jwtIssuer = jwtSection["Issuer"];
        var jwtAudience = jwtSection["Audience"];

        if (jwtKey == null || jwtIssuer == null || jwtAudience == null)
        {
            return null;
        }

        var security = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(security, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(jwtIssuer, 
            jwtAudience,
            claims,
            expires: DateTime.Now.AddMinutes(10), 
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}