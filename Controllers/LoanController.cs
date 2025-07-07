using System.Security.Claims;
using LoanBack.Models.Entities;
using LoanBack.Models.Requests;
using LoanBack.Models.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LoanBack.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoanController : ControllerBase
{
    private readonly ILoanRepository _repo;

    public LoanController(ILoanRepository repo)
    {
        _repo = repo;
    }

    [HttpPost("create")]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] LoanRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        Console.WriteLine(userIdStr);
        if (!int.TryParse(userIdStr, out var userId))
            return Unauthorized();

        var loan = new Loan
        {
            UserId = userId,
            LoanTypeId = request.LoanTypeId,
            StatusId = 1, // e.g. "New"
            Amount = request.Amount,
            CurrencyId = request.CurrencyId,
            MonthsTerm = request.MonthsTerm,
            CreatedAt = DateTime.UtcNow
        };

        var loanId = await _repo.CreateAsync(loan);

        return Ok(new LoanCreationResponse
        {
            LoanId = loanId
        });
    }



    [HttpGet("all")]
    [Authorize]
    public async Task<IActionResult> GetMyLoans()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdStr, out var userId))
            return Unauthorized();

        var loans = await _repo.GetByUserIdAsync(userId);
        return Ok(loans);
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetById(int id)
    {
        var loan = await _repo.GetByIdAsync(id);
        if (loan == null)
            return NotFound();

        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdStr, out var userId))
            return Unauthorized();

        if (loan.UserId != userId)
            return Forbid(); // or Ok if admin allowed

        return Ok(loan);
    }

    [HttpPut("{id}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateStatus(int id, [FromQuery] int statusId)
    {
        await _repo.UpdateStatusAsync(id, statusId);
        return Ok(new { message = "Status updated." });
    }


    [HttpGet("types")]
    [Authorize]
    public async Task<IActionResult> GetLoanTypes()
    {
        var types = await _repo.GetLoanTypesAsync();
        return Ok(types);
    }

    [HttpGet("currencies")]
    [Authorize]
    public async Task<IActionResult> GetLoanCurrencies()
    {
        var types = await _repo.GetLoanCurrenciesAsync();
        return Ok(types);
    }

    [HttpGet("statuses")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetLoanStatuses()
    {
        var statuses = await _repo.GetLoanStatusesAsync();
        return Ok(statuses);
    }
}

