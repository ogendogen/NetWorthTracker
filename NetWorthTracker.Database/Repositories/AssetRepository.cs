using FluentResults;
using Microsoft.EntityFrameworkCore;
using NetWorthTracker.Database.Models;

namespace NetWorthTracker.Database.Repositories;

public interface IAssetRepository
{
    Task<Result<Asset>> AddAssetToEntry(Asset asset, Entry entry, CancellationToken cancellationToken);
    Task<Result<Asset>> UpdateAsset(Asset asset, CancellationToken cancellationToken);
    Task<Result> RemoveAsset(Asset asset, CancellationToken cancellationToken);
}

public class AssetRepository : IAssetRepository
{
    private readonly NetWorthTrackerDbContext _context;
    public AssetRepository(NetWorthTrackerDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Asset>> AddAssetToEntry(Asset asset, Entry entry, CancellationToken cancellationToken)
    {
        asset.EntryId = entry.Id;
        await _context.Assets.AddAsync(asset, cancellationToken);
        int affected = await _context.SaveChangesAsync(cancellationToken);
        if (affected == 0)
        {
            return Result.Fail("Asset not added");
        }
        return asset;
    }

    public async Task<Result> RemoveAsset(Asset asset, CancellationToken cancellationToken)
    {
        _context.Remove(asset);
        int affected = await _context.SaveChangesAsync(cancellationToken);
        if (affected == 0)
        {
            return Result.Fail("Asset not removed");
        }
        return Result.Ok();
    }

    public async Task<Result<Asset>> UpdateAsset(Asset asset, CancellationToken cancellationToken)
    {
        _context.Assets.Update(asset);
        int affected = await _context.SaveChangesAsync(cancellationToken);
        if (affected == 0)
        {
            return Result.Fail("Asset not updated");
        }
        return Result.Ok(asset);
    }
}
