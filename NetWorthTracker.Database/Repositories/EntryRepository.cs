using FluentResults;
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

    public async Task<Result<Entry>> AddEntry(Entry entry, CancellationToken cancellationToken = default)
    {
        await _context.Entries.AddAsync(entry, cancellationToken);
        int affected = await _context.SaveChangesAsync(cancellationToken);
        if (affected == 0)
        {
            return Result.Fail("Entry not added");
        }

        return entry;
    }

    public async Task<Result<IEnumerable<Entry>>> GetUserEntries(int userId, CancellationToken cancellationToken = default)
    {
        var entries = await _context.Entries.Where(x => x.UserId == userId).ToListAsync(cancellationToken);
        return Result.Ok(entries.AsEnumerable());
    }

    public async Task<Result<int>> RemoveEntry(int entryId, CancellationToken cancellationToken = default)
    {
        var entry = await _context.Entries.FindAsync(entryId, cancellationToken);
        if (entry is null)
        {
            return Result.Fail("Entry not found");
        }

        _context.Remove(entry);
        int affected = await _context.SaveChangesAsync(cancellationToken);
        return Result.Ok(affected);
    }

    public async Task<Result<Entry>> UpdateEntry(Entry entry, CancellationToken cancellationToken = default)
    {
        _context.Entries.Update(entry);
        int affected = await _context.SaveChangesAsync(cancellationToken);

        if (affected == 0)
        {
            return Result.Fail("No entries updated");
        }

        return Result.Ok(entry);
    }
}
