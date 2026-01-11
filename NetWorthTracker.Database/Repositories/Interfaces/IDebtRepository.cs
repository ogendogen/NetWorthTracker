using FluentResults;
using NetWorthTracker.Database.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetWorthTracker.Database.Repositories.Interfaces;

public interface IDebtRepository
{
    Task<Result<Debt>> AddDebtToEntry(Debt debt, Entry entry, CancellationToken cancellationToken);
    Task<Result<Debt>> UpdateDebt(Debt debt, CancellationToken cancellationToken);
    Task<Result> RemoveDebt(Debt debt, CancellationToken cancellationToken);
}
