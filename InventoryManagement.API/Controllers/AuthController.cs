using InventoryManagement.API.Authentication;
using InventoryManagement.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(UserManager<ApplicationUser> userManager, TokenService tokenService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName
        };

        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded) return BadRequest(result.Errors);

        return Ok("User created successfully!");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user == null || !await userManager.CheckPasswordAsync(user, request.Password))
            return Unauthorized("Invalid credentials.");

        var token = await tokenService.GenerateAccessTokenAsync(user);
        return Ok(new { Token = token });
    }
}