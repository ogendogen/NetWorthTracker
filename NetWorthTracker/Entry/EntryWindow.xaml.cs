using System.Windows;

namespace NetWorthTracker.Entry;

/// <summary>
/// Interaction logic for EntryWindow.xaml
/// </summary>
public partial class EntryWindow : Window
{
    public EntryWindow(IEntryWindowViewModel entryWindowViewModel)
    {
        InitializeComponent();
        DataContext = entryWindowViewModel;
    }
}
