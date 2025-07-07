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
                loan.UserId,
                loan.LoanTypeId,
                loan.StatusId,
                loan.Amount,
                loan.Currency,
                loan.MonthlyPeriod,
                loan.CreatedAt
            },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<Loan>> GetByUserIdAsync(int userId)
    {
        using var conn = CreateConnection();
        var result = await conn.QueryAsync<Loan, LoanType, LoanStatus, Loan>(
            "sp_GetLoansByUserId",
            (loan, type, status) =>
            {
                loan.LoanType = type;
                loan.LoanStatus = status;
                return loan;
            },
            new { p_UserId = userId },
            splitOn: "LoanTypeId,StatusId",
            commandType: CommandType.StoredProcedure);

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

}

