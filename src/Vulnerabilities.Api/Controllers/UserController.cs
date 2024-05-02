
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Vulnerabilities.Api.Controllers;

[ApiController]
[Route("user")]
public class UserController(SignInManager<IdentityUser> signInManager, IConfiguration config) : ControllerBase
{
    private readonly SignInManager<IdentityUser> _signInManager = signInManager;
    private readonly IConfiguration _config = config;

    [HttpPost("signin")]
    public async Task<IActionResult> SignIn([FromBody] SignInModel model)
    {
        var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, false, false);

        if (result.Succeeded)
        {
            var user = await _signInManager.UserManager.FindByNameAsync(model.Username);
            var roles = await _signInManager.UserManager.GetRolesAsync(user!);
            var token = GenerateJwtToken(user!, roles);

            // Remove Identity's automatically generated cookies
            Response.Cookies.Delete(".AspNetCore.Identity.Application");
            Response.Headers.Remove("Set-Cookie");
            return Ok(new { AccessToken = token });

        }

        return Unauthorized();
    }

    private string GenerateJwtToken(IdentityUser user, IList<string> roles)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        var key = Encoding.UTF8.GetBytes(_config["Jwt:Secret"]!);
        var claims = new List<Claim> {
            new(ClaimTypes.Name, user.UserName!),
            new(ClaimTypes.NameIdentifier, user.Id!),
        };
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(20),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256),
            Issuer = _config["Jwt:Issuer"],
            Audience = _config["Jwt:Audience"],
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}

public class SignInModel
{
    public required string Username { get; set; }
    public required string Password { get; set; }
}
