using NetWorthTracker.Database.Models;

namespace NetWorthTracker.Database.Repositories.Interfaces;

public interface IEntryRepository
{
    Task<IEnumerable<Entry>> GetUserEntries(int userId, CancellationToken cancellationToken = default);
    Task<Entry> AddEntry(Entry entry, CancellationToken cancellationToken = default);
    Task<Entry> UpdateEntry(Entry entry, CancellationToken cancellationToken = default);
    Task<bool> RemoveEntry(int entryId, CancellationToken cancellationToken = default);
}
