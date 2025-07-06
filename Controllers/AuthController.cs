using Microsoft.AspNetCore.Mvc;
using LoanBack.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using LoanBack.Models.Responses;

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
    public IActionResult CheckAuth()
    {
        var email = User.Claims
            .FirstOrDefault(c => c.Type.Split('/').Last() == "emailaddress")
            ?.Value;
        var role = User.Claims
            .FirstOrDefault(c => c.Type.Split('/').Last() == "role")
            ?.Value;
        return Ok(new AuthCheck
        {
            Email = email ?? throw new Exception("Email not found"),
            Role = role ?? throw new Exception("Role not found")
        });
    }
}

