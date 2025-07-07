namespace LoanBack.Models.Entities;

public class Loan
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int LoanTypeId { get; set; }
    public int StatusId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public int MonthlyPeriod { get; set; }
    public DateTime CreatedAt { get; set; }

    public LoanType? LoanType { get; set; }
    public LoanStatus? LoanStatus { get; set; }
}

