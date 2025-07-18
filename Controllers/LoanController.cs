using System.Security.Claims;
using LoanBack.Enums;
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
    private readonly IRabbitMqPublisher _mq;
    private readonly ILoanQueueConsumer _loanQueue;

    public LoanController(ILoanRepository repo, IRabbitMqPublisher mq, ILoanQueueConsumer loanQueue)
    {
        _repo = repo;
        _mq = mq;
        _loanQueue = loanQueue;
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
            LoanTypeId = (LoanTypeEnum)request.LoanTypeId,
            StatusId = LoanStatusEnum.New,
            Amount = request.Amount,
            CurrencyId = (CurrencyEnum)request.CurrencyId,
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

    [HttpPut("update")]
    [Authorize]
    public async Task<IActionResult> Update([FromBody] LoanRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (request.Id <= 0 || request.Id == null)
            return BadRequest(new { error = "Loan ID is required for update." });

        await ValidateLoanOwnership(request.Id.Value, LoanStatusEnum.New);

        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdStr, out var userId))
            return Unauthorized();

        var loan = new Loan
        {
            Id = request.Id.Value,
            UserId = userId,
            LoanTypeId = (LoanTypeEnum)request.LoanTypeId,
            Amount = request.Amount,
            CurrencyId = (CurrencyEnum)request.CurrencyId,
            MonthsTerm = request.MonthsTerm
        };

        var updatedLoanId = await _repo.UpdateAsync(loan);
        return Ok(new LoanCreationResponse { LoanId = updatedLoanId, Message = "Loan updated successfully" });
    }


    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        await ValidateLoanOwnership(id);

        var success = await _repo.DeleteAsync(id);
        if (!success)
            return NotFound(new { error = "Loan not found or could not be deleted." });

        return Ok(new LoanDeletionResponse { Deleted = success, LoanId = id });
    }

    [HttpPut("{id}/send-loan-request")]
    [Authorize]
    public async Task<IActionResult> SendLoanRequest(int id)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdStr, out var userId))
            return Unauthorized();

        await _repo.UpdateStatusAsync(id, LoanStatusEnum.Requested);

        _mq.PublishLoanRequest(id); // 🔁 Send to RabbitMQ

        return Ok(new LoanCreationResponse { LoanId = id, Message = "Loan updated successfully" });
    }

    [HttpPut("{id}/approve-loan")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ApproveLoanRequest(int id)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdStr, out var userId))
            return Unauthorized();

        await _repo.UpdateStatusAsync(id, LoanStatusEnum.Confirmed);

        return Ok(new LoanCreationResponse { LoanId = id, Message = "Loan updated successfully" });
    }


    [HttpPut("{id}/reject-loan")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RejectLoanRequest(int id)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdStr, out var userId))
            return Unauthorized();

        await _repo.UpdateStatusAsync(id, LoanStatusEnum.Rejected);

        return Ok(new LoanCreationResponse { LoanId = id, Message = "Loan updated successfully" });
    }

    [HttpGet("admin/pending-loans")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetPendingLoansFromQueue()
    {
        var loanIds = await _loanQueue.GetLoanIdsFromQueueAsync();
        var loansToReturn = new List<Loan>();

        foreach (var id in loanIds)
        {
            var loan = await _repo.GetByIdAsync(id);
            if (loan != null && (LoanStatusEnum)loan.StatusId == LoanStatusEnum.Requested)
            {
                await _repo.UpdateStatusAsync(id, LoanStatusEnum.UnderReview);
                loan.StatusId = LoanStatusEnum.UnderReview;
                loan.LoanStatus = new LoanStatus
                {
                    Id = (int)LoanStatusEnum.UnderReview,
                    Name = "დამუშავების პროცესში"
                };
                loansToReturn.Add(loan);
            }
        }

        return Ok(loansToReturn);
    }

    [HttpGet("admin/under-review-loans")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetUnderReviewLoans()
    {
        var loans = await _repo.GetLoansByStatusAsync(LoanStatusEnum.UnderReview);
        return Ok(loans);
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


    private async Task<(bool IsValid, IActionResult? ErrorResult, Loan? Loan)> ValidateLoanOwnership(
        int loanId,
        LoanStatusEnum? expectedStatus = null)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdStr, out var userId))
            return (false, Unauthorized(), null);

        var loan = await _repo.GetByIdAsync(loanId);
        if (loan == null)
            return (false, NotFound(new { error = "Loan not found." }), null);

        if (loan.UserId != userId)
            return (false, BadRequest(new { error = "Loan not found or not yours." }), null);

        if (expectedStatus.HasValue && (LoanStatusEnum)loan.StatusId != expectedStatus)
            return (false, BadRequest(new { error = "Loan is not in the expected status." }), null);

        return (true, null, loan);
    }
}

