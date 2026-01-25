using System;
using System.Collections.Generic;
using System.Text;

namespace NetWorthTracker.Database.Models;

public class Debt
{
    public int Id { get; set; }
    public required string Name { get; set; } = string.Empty;
    public int EntryId { get; set; }
    public decimal Value { get; set; } = 0;
    public virtual Entry Entry { get; set; } = new Entry();
}
