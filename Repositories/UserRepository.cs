using System.Data;
using Dapper;
using LoanBack.Models.Entities;
using MySqlConnector;

namespace LoanBack.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IConfiguration _config;
    private readonly string? _conn;

    public UserRepository(IConfiguration config)
    {
        _config = config;
        _conn = _config.GetConnectionString("DefaultConnection");
    }

    private IDbConnection CreateConnection() => new MySqlConnection(_conn);


    public async Task<User?> GetByEmailAsync(string email)
    {
        using var db = CreateConnection();
        return await db.QueryFirstOrDefaultAsync<User>(
            "sp_GetUserByEmail",
            new { userEmail = email },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        using var db = CreateConnection();
        var count = await db.ExecuteScalarAsync<int>(
            "sp_EmailExists",
            new { userEmail = email },
            commandType: CommandType.StoredProcedure);
        return count > 0;
    }

    public async Task<int> CreateAsync(User user)
    {
        using var db = CreateConnection();
        var parameters = new
        {
            firstName = user.FirstName,
            lastName = user.LastName,
            idNumber = user.IdNumber,
            dateOfBirth = user.DateOfBirth,
            email = user.Email,
            passwordHash = user.PasswordHash,
            role = user.Role,
            createdAt = user.CreatedAt
        };

        return await db.ExecuteScalarAsync<int>(
            "sp_CreateUser",
            parameters,
            commandType: CommandType.StoredProcedure);
    }

}

