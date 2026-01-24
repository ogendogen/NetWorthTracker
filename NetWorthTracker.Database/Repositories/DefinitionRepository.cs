using FluentResults;
using Microsoft.EntityFrameworkCore;
using NetWorthTracker.Database.Models;
using NetWorthTracker.Database.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetWorthTracker.Database.Repositories;

public class DefinitionRepository : IDefinitionRepository
{
    private readonly NetWorthTrackerDbContext _context;
    public DefinitionRepository(NetWorthTrackerDbContext context)
    {
        _context = context;
    }
    
    public async Task<Result<IEnumerable<Definition>>> GetDefinitionsByUserId(int userId, DefinitionType definitionType, CancellationToken cancellationToken = default)
    {
        var assetDefinitions = await _context.Definitions.Where(x => x.UserId == userId && x.Type == definitionType).ToListAsync(cancellationToken);
        return Result.Ok(assetDefinitions.AsEnumerable());
    }

    public async Task<Result> SyncUserDefinitions(User user, IEnumerable<Definition> definitions, DefinitionType definitionType, CancellationToken cancellationToken = default)
    {
        definitions = definitions.Select(def => new Definition
        {
            Name =  def.Name,
            User = user,
            UserId = user.Id,
            Type = definitionType
        });

        var existing = await _context.Definitions.Where(x => x.UserId == user.Id && x.Type == definitions.First().Type).ToListAsync(cancellationToken);
        
        _context.Definitions.RemoveRange(existing);
        await _context.Definitions.AddRangeAsync(definitions, cancellationToken);
        
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Ok();
    }
}
