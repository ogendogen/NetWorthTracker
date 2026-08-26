namespace NetWorthTracker.Domain.User.Interfaces;

public interface IUserRepository
{
    Task<bool> LoginAsync(
        string username,
        string password,
        CancellationToken cancellationToken = default);

    Task<bool> RegisterAsync(
        string username,
        string password,
        string email,
        CancellationToken cancellationToken = default);

    Task<Models.User?> GetByUsernameAsync(
        string username,
        string email,
        CancellationToken cancellationToken = default);
}