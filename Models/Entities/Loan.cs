using LoanBack.Enums;

namespace LoanBack.Models.Entities;

public class Loan
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public LoanTypeEnum LoanTypeId { get; set; }
    public LoanStatusEnum StatusId { get; set; }
    public decimal Amount { get; set; }
    public CurrencyEnum CurrencyId { get; set; }
    public int MonthsTerm { get; set; }
    public DateTime CreatedAt { get; set; }

    public Currency? Currency { get; set; }
    public LoanType? LoanType { get; set; }
    public LoanStatus? LoanStatus { get; set; }
}

