using Microsoft.EntityFrameworkCore;
using NetWorthTracker.Database.Models;
using NetWorthTracker.Database.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetWorthTracker.Database.Repositories;

public class EntryRepository : IEntryRepository
{
    private readonly NetWorthTrackerDbContext _context;
    public EntryRepository(NetWorthTrackerDbContext context)
    {
        _context = context;
    }

    public async Task<Entry> AddEntry(Entry entry, CancellationToken cancellationToken = default)
    {
        await _context.Entry.AddAsync(entry, cancellationToken);
        int affected = await _context.SaveChangesAsync(cancellationToken);
        return entry;
    }

    public async Task<IEnumerable<Entry>> GetUserEntries(int userId, CancellationToken cancellationToken = default)
    {
        return await _context.Entry.Where(x => x.UserId == userId).ToListAsync(cancellationToken: cancellationToken);
    }

    public Task<bool> RemoveEntry(int entryId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Entry> UpdateEntry(Entry entry, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
