using FluentResults;
using Microsoft.EntityFrameworkCore;
using NetWorthTracker.Database.Models;
using NetWorthTracker.Database.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetWorthTracker.Database.Repositories;

public class DebtRepository : IDebtRepository
{
    private readonly NetWorthTrackerDbContext _context;
    public DebtRepository(NetWorthTrackerDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Debt>> AddDebtToEntry(Debt debt, Entry entry, CancellationToken cancellationToken)
    {
        debt.EntryId = entry.Id;
        await _context.Debts.AddAsync(debt, cancellationToken);
        int affected = await _context.SaveChangesAsync(cancellationToken);
        if (affected == 0)
        {
            return Result.Fail("Debt not added");
        }
        return debt;
    }

    public async Task<Result> RemoveDebt(Debt debt, CancellationToken cancellationToken)
    {
        _context.Remove(debt);
        int affected = await _context.SaveChangesAsync(cancellationToken);
        if (affected == 0)
        {
            return Result.Fail("Debt not removed");
        }
        return Result.Ok();
    }

    public async Task<Result<Debt>> UpdateDebt(Debt debt, CancellationToken cancellationToken)
    {
        _context.Debts.Update(debt);
        int affected = await _context.SaveChangesAsync(cancellationToken);
        if (affected == 0)
        {
            return Result.Fail("Debt not updated");
        }
        return Result.Ok(debt);
    }
}
