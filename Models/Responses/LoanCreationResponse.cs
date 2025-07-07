namespace LoanBack.Models.Responses;

public class LoanCreationResponse
{
    public int LoanId { get; set; }
    public string Message { get; set; } = "Loan created successfully";
}
