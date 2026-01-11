using System;
using System.Collections.Generic;
using System.Text;

namespace NetWorthTracker.Database.Models;

public class AssetDefinition
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public virtual User User { get; set; } = new User();
    public string Name { get; set; } = string.Empty;
}
