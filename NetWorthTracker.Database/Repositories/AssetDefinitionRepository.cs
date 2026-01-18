using FluentResults;
using Microsoft.EntityFrameworkCore;
using NetWorthTracker.Database.Models;
using NetWorthTracker.Database.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetWorthTracker.Database.Repositories;

public class AssetDefinitionRepository : IAssetDefinitionRepository
{
    private readonly NetWorthTrackerDbContext _context;
    public AssetDefinitionRepository(NetWorthTrackerDbContext context)
    {
        _context = context;
    }

    public async Task<Result<AssetDefinition>> AddAssetDefinition(AssetDefinition assetDefinition, CancellationToken cancellationToken = default)
    {
        await _context.AssetsDefinitions.AddAsync(assetDefinition, cancellationToken);
        int affected = await _context.SaveChangesAsync(cancellationToken);
        if (affected == 0)
        {
            return Result.Fail("AssetDefinition not added");
        }
        return assetDefinition;
    }

    public async Task<Result<AssetDefinition>> UpdateAssetDefinition(AssetDefinition assetDefinition, CancellationToken cancellationToken = default)
    {
        _context.AssetsDefinitions.Update(assetDefinition);
        int affected = await _context.SaveChangesAsync(cancellationToken);
        if (affected == 0)
        {
            return Result.Fail("AssetDefinition not updated");
        }
        return Result.Ok(assetDefinition);
    }

    public async Task<Result> RemoveAssetDefinition(int assetDefinitionId, CancellationToken cancellationToken = default)
    {
        var assetDefinition = await _context.AssetsDefinitions.FindAsync(assetDefinitionId, cancellationToken);
        if (assetDefinition is null)
        {
            return Result.Fail("AssetDefinition not found");
        }
        _context.Remove(assetDefinition);
        int affected = await _context.SaveChangesAsync(cancellationToken);
        if (affected == 0)
        {
            return Result.Fail("AssetDefinition not removed");
        }
        return Result.Ok();
    }

    public async Task<Result<IEnumerable<AssetDefinition>>> GetByUserId(int userId, CancellationToken cancellationToken = default)
    {
        var assetDefinitions = await _context.AssetsDefinitions.Where(x => x.UserId == userId).ToListAsync(cancellationToken);
        return Result.Ok(assetDefinitions.AsEnumerable());
    }

    public async Task<Result> SyncUserDefinitions(int userId, IEnumerable<AssetDefinition> definitions, CancellationToken cancellationToken = default)
    {
        definitions = definitions.Select(def => new AssetDefinition() { Name =  def.Name, User = def.User, UserId = def.UserId});
        var existing = await _context.AssetsDefinitions.Where(x => x.UserId == userId).ToListAsync(cancellationToken);
        
        _context.AssetsDefinitions.RemoveRange(existing);
        await _context.AssetsDefinitions.AddRangeAsync(definitions, cancellationToken);
        
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Ok();
    }
}
