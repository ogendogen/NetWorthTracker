using FluentResults;
using NetWorthTracker.Database.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetWorthTracker.Database.Repositories.Interfaces;

public interface IDefinitionRepository
{
    Task<Result<IEnumerable<Definition>>> GetDefinitionsByUserId(int userId, DefinitionType definitionType, CancellationToken cancellationToken = default);
    Task<Result> SyncUserDefinitions(int userId, IEnumerable<Definition> definitions, CancellationToken cancellationToken = default);
}
