using FluentResults;
using NetWorthTracker.Database.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetWorthTracker.Database.Repositories.Interfaces;

public interface IAssetDefinitionRepository
{
    Task<Result<AssetDefinition>> AddAssetDefinition(AssetDefinition assetDefinition, CancellationToken cancellationToken = default);
    Task<Result<AssetDefinition>> UpdateAssetDefinition(AssetDefinition assetDefinition, CancellationToken cancellationToken = default);
    Task<Result> RemoveAssetDefinition(int assetDefinitionId, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<AssetDefinition>>> GetByUserId(int userId, CancellationToken cancellationToken = default);
}
