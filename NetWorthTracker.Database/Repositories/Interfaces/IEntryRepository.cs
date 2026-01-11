using FluentResults;
using NetWorthTracker.Database.Models;

namespace NetWorthTracker.Database.Repositories.Interfaces;

public interface IEntryRepository
{
    Task<Result<IEnumerable<Entry>>> GetUserEntries(int userId, CancellationToken cancellationToken = default);
    Task<Result<Entry>> AddEntry(Entry entry, CancellationToken cancellationToken = default);
    Task<Result<Entry>> UpdateEntry(Entry entry, CancellationToken cancellationToken = default);
    Task<Result<int>> RemoveEntry(int entryId, CancellationToken cancellationToken = default);
}
