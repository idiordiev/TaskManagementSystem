namespace TaskManagementSystem.Application.Interfaces;

public interface IIdentityService
{
    Task CreateAccountAsync(string email, string password, int userId);
    Task<string> GetTokenAsync(string email, string password);
    Task DeleteAccountsForUserAsync(int userId, CancellationToken cancellationToken = default);
    Task TryGrantAdminRightsAsync(int userId, CancellationToken cancellationToken = default);
}