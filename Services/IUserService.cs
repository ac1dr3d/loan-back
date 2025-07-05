using LoanBack.Models.Requests;

public interface IUserService
{
    Task RegisterAsync(RegisterRequest request);
    Task<string> LoginAsync(LoginRequest request);
}

