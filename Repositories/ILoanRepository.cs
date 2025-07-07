using LoanBack.Models.Entities;

public interface ILoanRepository
{
    Task<int> CreateAsync(Loan loan);
    Task<IEnumerable<Loan>> GetByUserIdAsync(int userId);
    Task<Loan?> GetByIdAsync(int loanId);
    Task UpdateStatusAsync(int loanId, int newStatusId);
    Task<IEnumerable<LoanType>> GetLoanTypesAsync();
    Task<IEnumerable<LoanStatus>> GetLoanStatusesAsync();
    Task<IEnumerable<Currency>> GetLoanCurrenciesAsync();
}

