namespace LoanBack.Models.Responses;

public class LoanDeletionResponse
{
    public bool Deleted { get; set; }
    public int LoanId { get; set; }
    public string Message { get; set; } = "Loan deleted successfully";
}
