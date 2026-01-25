using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetWorthTracker.Entry;

public interface IEntryWindowViewModel
{
}

public class EntryWindowViewModel : IEntryWindowViewModel
{
    public ISeries[] Series { get; set; } =
    {
        new LineSeries<double>
        {
            Values = new double[] { 2, 5, 4, 6, 8 }
        }
    };

}
