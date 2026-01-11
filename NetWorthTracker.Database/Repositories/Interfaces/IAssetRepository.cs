using FluentResults;
using NetWorthTracker.Database.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetWorthTracker.Database.Repositories.Interfaces;

public interface IAssetRepository
{
    Task<Result<Asset>> AddAssetToEntry(Asset asset, Entry entry, CancellationToken cancellationToken);
    Task<Result<Asset>> UpdateAsset(Asset asset, CancellationToken cancellationToken);
    Task<Result> RemoveAsset(Asset asset, CancellationToken cancellationToken);
}
