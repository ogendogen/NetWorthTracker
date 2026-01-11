using System;
using System.Collections.Generic;
using System.Text;

namespace NetWorthTracker.Database.Models;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public virtual ICollection<Entry> Entries { get; set; } = new List<Entry>();
    public virtual ICollection<AssetDefinition> AssetsDefinitions { get; set; } = new List<AssetDefinition>();
    public virtual ICollection<DebtDefinition> DebtsDefinitions { get; set; } = new List<DebtDefinition>();
}
