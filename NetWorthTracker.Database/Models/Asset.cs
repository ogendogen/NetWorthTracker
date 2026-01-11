using System;
using System.Collections.Generic;
using System.Text;

namespace NetWorthTracker.Database.Models;

public class Asset
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int EntryId { get; set; }
    public virtual Entry Entry { get; set; } = new Entry();
}
