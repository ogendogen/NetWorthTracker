using FluentResults;
using Microsoft.EntityFrameworkCore;
using NetWorthTracker.Database.Models;
using NetWorthTracker.Database.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetWorthTracker.Database.Repositories;

public class DebtDefinitionRepository : IDebtDefinitionRepository
{
    private readonly NetWorthTrackerDbContext _context;
    public DebtDefinitionRepository(NetWorthTrackerDbContext context)
    {
        _context = context;
    }

    public async Task<Result<DebtDefinition>> AddDebtDefinition(DebtDefinition debtDefinition, CancellationToken cancellationToken = default)
    {
        await _context.DebtsDefinitions.AddAsync(debtDefinition, cancellationToken);
        int affected = await _context.SaveChangesAsync(cancellationToken);
        if (affected == 0)
        {
            return Result.Fail("DebtDefinition not added");
        }
        return debtDefinition;
    }

    public async Task<Result<DebtDefinition>> UpdateDebtDefinition(DebtDefinition debtDefinition, CancellationToken cancellationToken = default)
    {
        _context.DebtsDefinitions.Update(debtDefinition);
        int affected = await _context.SaveChangesAsync(cancellationToken);
        if (affected == 0)
        {
            return Result.Fail("DebtDefinition not updated");
        }
        return Result.Ok(debtDefinition);
    }

    public async Task<Result> RemoveDebtDefinition(int debtDefinitionId, CancellationToken cancellationToken = default)
    {
        var debtDefinition = await _context.DebtsDefinitions.FindAsync(debtDefinitionId, cancellationToken);
        if (debtDefinition is null)
        {
            return Result.Fail("DebtDefinition not found");
        }
        _context.Remove(debtDefinition);
        int affected = await _context.SaveChangesAsync(cancellationToken);
        if (affected == 0)
        {
            return Result.Fail("DebtDefinition not removed");
        }
        return Result.Ok();
    }

    public async Task<Result<IEnumerable<DebtDefinition>>> GetByUserId(int userId, CancellationToken cancellationToken = default)
    {
        var debtDefinitions = await _context.DebtsDefinitions.Where(x => x.UserId == userId).ToListAsync(cancellationToken);
        return Result.Ok(debtDefinitions.AsEnumerable());
    }
}
