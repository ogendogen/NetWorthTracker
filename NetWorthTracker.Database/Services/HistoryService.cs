using FluentResults;
using Microsoft.EntityFrameworkCore;
using NetWorthTracker.Database.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace NetWorthTracker.Database.Services;

public interface IHistoryService
{
    Task<Result<History>> GetAssetHistory(string assetName, int userId, CancellationToken cancellationToken = default);
    Task<Result<History>> GetDebtHistory(string assetName, int userId, CancellationToken cancellationToken = default);
}

public class HistoryService : IHistoryService
{
    private readonly NetWorthTrackerDbContext _dbContext;

    public HistoryService(NetWorthTrackerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<History>> GetAssetHistory(string assetName, int userId, CancellationToken cancellationToken)
    {
        var query = _dbContext.Assets
            .Where(a => a.Name == assetName && a.Entry.UserId == userId)
            .OrderBy(a => a.Entry.Date)
            .Select(a => new { a.Value, a.Entry.Date });

        var sql = query.ToQueryString();
        Console.WriteLine(sql);

        var history = await query.ToListAsync(cancellationToken);

        return Result.Ok(new History
        {
            Items = history.Select(h => new HistoryItem { Value = h.Value, Date = h.Date }),
        });
    }

    public async Task<Result<History>> GetDebtHistory(string assetName, int userId, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Debts
            .Where(a => a.Name == assetName && a.Entry.UserId == userId)
            .OrderBy(a => a.Entry.Date)
            .Select(a => new { a.Value, a.Entry.Date });

        var sql = query.ToQueryString();
        Console.WriteLine(sql);

        var history = await query.ToListAsync(cancellationToken);

        return Result.Ok(new History
        {
            Items = history.Select(h => new HistoryItem { Value = h.Value, Date = h.Date }),
        });
    }
}