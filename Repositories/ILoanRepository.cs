using LoanBack.Enums;
using LoanBack.Models.Entities;

public interface ILoanRepository
{
    Task<int> CreateAsync(Loan loan);
    Task<IEnumerable<Loan>> GetByUserIdAsync(int userId);
    Task<IEnumerable<Loan>> GetLoansByStatusAsync(LoanStatusEnum statusId);
    Task<Loan?> GetByIdAsync(int loanId);
    Task UpdateStatusAsync(int loanId, LoanStatusEnum newStatus);
    Task<IEnumerable<LoanType>> GetLoanTypesAsync();
    Task<IEnumerable<LoanStatus>> GetLoanStatusesAsync();
    Task<IEnumerable<Currency>> GetLoanCurrenciesAsync();
    Task<int> UpdateAsync(Loan loan);
    Task<bool> DeleteAsync(int loanId);
}

