namespace NetWorthTracker.Domain.User.Interfaces;

public interface IUserRepository
{
    Task<bool> LoginAsync(
        string username,
        string password,
        CancellationToken cancellationToken = default);
}