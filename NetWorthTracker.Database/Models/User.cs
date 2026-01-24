using System;
using System.Collections.Generic;
using System.Text;

namespace NetWorthTracker.Database.Models;

public class User
{
    public int Id { get; set; }
    public required string Name { get; set; } = string.Empty;
    public virtual ICollection<Entry> Entries { get; set; } = new List<Entry>();
    public virtual ICollection<Definition> Definitions { get; set; } = new List<Definition>();

    public override string? ToString()
    {
        return Name;
    }
}
