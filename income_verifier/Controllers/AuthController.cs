using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using income_verifier.DTOs.AppUser;
using income_verifier.Examples.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using income_verifier.Models;
using income_verifier.Repositories.Interfaces;
using Swashbuckle.AspNetCore.Filters;

namespace income_verifier.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(
    IConfiguration config, 
    IUserRepository users
) : ControllerBase
{
    
    [HttpPost("login")]
    [SwaggerRequestExample(typeof(LoginRequestDto), typeof(LoginRequestDtoExample))]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
    {
        var user = await users.GetByUsernameAsync(dto.Username);
        if (user == null || user.Password != dto.Password)
            return Unauthorized();

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.IsAdmin ? "Admin" : "User")
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            config["Jwt:Key"] ?? "super-secret-key-123456789!"
        ));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: config["Jwt:Issuer"] ?? "income-verifier-app",
            audience: config["Jwt:Issuer"] ?? "income-verifier-app",
            claims: claims,
            expires: DateTime.Now.AddHours(2),
            signingCredentials: creds
        );

        return Ok(new
        {
            token = new JwtSecurityTokenHandler().WriteToken(token)
        });
    }

    [HttpPost("register")]
    [SwaggerRequestExample(typeof(RegisterRequestDto), typeof(RegisterRequestDtoExample))]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto dto)
    {
        var exists = await users.GetByUsernameAsync(dto.Username);
        if (exists != null)
            return Conflict("Username already exists");

        var user = new User
        {
            Username = dto.Username,
            Password = dto.Password,
            IsAdmin = dto.IsAdmin
        };

        await users.AddAsync(user);
        return Ok("Registered successfully!");
    }
}
