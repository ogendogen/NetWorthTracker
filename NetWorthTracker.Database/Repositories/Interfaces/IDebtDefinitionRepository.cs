using FluentResults;
using NetWorthTracker.Database.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetWorthTracker.Database.Repositories.Interfaces;

public interface IDebtDefinitionRepository
{
    Task<Result<DebtDefinition>> AddDebtDefinition(DebtDefinition debtDefinition, CancellationToken cancellationToken = default);
    Task<Result<DebtDefinition>> UpdateDebtDefinition(DebtDefinition debtDefinition, CancellationToken cancellationToken = default);
    Task<Result> RemoveDebtDefinition(int debtDefinitionId, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<DebtDefinition>>> GetByUserId(int userId, CancellationToken cancellationToken = default);
}
