using Dapper;
using LoanBack.Models.Entities;
using MySqlConnector;
using System.Data;

namespace LoanBack.Repositories;

public class LoanRepository : ILoanRepository
{
    private readonly IConfiguration _config;
    private readonly string? _conn;

    public LoanRepository(IConfiguration config)
    {
        _config = config;
        _conn = _config.GetConnectionString("DefaultConnection");
    }

    private IDbConnection CreateConnection() => new MySqlConnection(_conn);


    public async Task<int> CreateAsync(Loan loan)
    {
        using var conn = CreateConnection();
        return await conn.ExecuteScalarAsync<int>(
            "sp_CreateLoan",
            new
            {
                p_UserId = loan.UserId,
                p_LoanTypeId = loan.LoanTypeId,
                p_StatusId = loan.StatusId,
                p_Amount = loan.Amount,
                p_CurrencyId = loan.CurrencyId,
                p_MonthsTerm = loan.MonthsTerm,
                p_CreatedAt = loan.CreatedAt
            },

            commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<Loan>> GetByUserIdAsync(int userId)
    {
        using var conn = CreateConnection();

        var result = await conn.QueryAsync<Loan, LoanType, LoanStatus, Currency, Loan>(
            "sp_GetLoansByUserId",
            (loan, type, status, currency) =>
            {
                loan.LoanType = type;
                loan.LoanStatus = status;
                loan.Currency = currency;
                return loan;
            },
            new { p_UserId = userId },
            splitOn: "LoanTypeId,StatusId,CurrencyId",
            commandType: CommandType.StoredProcedure
        );
        return result;
    }

    public async Task<Loan?> GetByIdAsync(int loanId)
    {
        using var conn = CreateConnection();
        var result = await conn.QueryAsync<Loan, LoanType, LoanStatus, Loan>(
            "sp_GetLoanById",
            (loan, type, status) =>
            {
                loan.LoanType = type;
                loan.LoanStatus = status;
                return loan;
            },
            new { p_LoanId = loanId },
            splitOn: "LoanTypeId,StatusId",
            commandType: CommandType.StoredProcedure);

        return result.FirstOrDefault();
    }

    public async Task UpdateStatusAsync(int loanId, int newStatusId)
    {
        using var conn = CreateConnection();
        await conn.ExecuteAsync(
            "sp_UpdateLoanStatus",
            new { p_LoanId = loanId, p_StatusId = newStatusId },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<LoanType>> GetLoanTypesAsync()
    {
        using var conn = CreateConnection();
        return await conn.QueryAsync<LoanType>(
            "sp_GetLoanTypes",
            commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<LoanStatus>> GetLoanStatusesAsync()
    {
        using var conn = CreateConnection();
        return await conn.QueryAsync<LoanStatus>(
            "sp_GetLoanStatuses",
            commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<Currency>> GetLoanCurrenciesAsync()
    {
        using var conn = CreateConnection();
        return await conn.QueryAsync<Currency>(
            "sp_GetCurrencies",
            commandType: CommandType.StoredProcedure);
    }

}

