using LoanBack.Models.Entities;


public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<bool> EmailExistsAsync(string email);
    Task<int> CreateAsync(User user);
}
