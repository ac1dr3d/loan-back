using Microsoft.AspNetCore.Mvc;
using LoanBack.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using LoanBack.Models.Responses;
using System.Security.Claims;

namespace LoanBack.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;

    public AuthController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        try
        {
            await _userService.RegisterAsync(request);
            return Ok(new { message = "User registered successfully." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            var token = await _userService.LoginAsync(request);
            return Ok(new { token });
        }
        catch (Exception ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
    }

    [HttpGet("check-auth")]
    [Authorize]
    public async Task<IActionResult> CheckAuth([FromServices] IUserRepository userRepo)
    {
        var email = User.FindFirstValue(ClaimTypes.Email);
        var role = User.FindFirstValue(ClaimTypes.Role);

        if (string.IsNullOrWhiteSpace(email))
            return Unauthorized("Email claim not found.");

        var user = await userRepo.GetByEmailAsync(email);
        if (user == null)
            return Unauthorized("User not found.");

        return Ok(new AuthCheck
        {
            Email = user.Email,
            Role = user.Role ?? role ?? "User"
        });
    }
}

