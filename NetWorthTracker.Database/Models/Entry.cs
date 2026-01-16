using System;
using System.Collections.Generic;
using System.Text;

namespace NetWorthTracker.Database.Models;

public class Entry
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public virtual User User { get; set; } = new User() { Name = string.Empty };
    public string? Name { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public DateTime Date { get; set; }
    public virtual ICollection<Asset> Assets { get; set; } = new List<Asset>();
    public virtual ICollection<Debt> Debts { get; set; } = new List<Debt>();
}
