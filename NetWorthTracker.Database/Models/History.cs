using System;
using System.Collections.Generic;
using System.Text;

namespace NetWorthTracker.Database.Models;

public class History
{
    public IEnumerable<HistoryItem> Items { get; set; }
}

public class HistoryItem
{
    public decimal Value { get; set; }
    public DateTime Date { get; set; }
}
