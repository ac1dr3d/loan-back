namespace LoanBack.Models.Entities;

public class Loan
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int LoanTypeId { get; set; }
    public int StatusId { get; set; }
    public decimal Amount { get; set; }
    public int CurrencyId { get; set; }
    public int MonthsTerm { get; set; }
    public DateTime CreatedAt { get; set; }

    public Currency? Currency { get; set; }
    public LoanType? LoanType { get; set; }
    public LoanStatus? LoanStatus { get; set; }
}

