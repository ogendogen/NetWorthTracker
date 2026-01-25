using NetWorthTracker.Database.Models;
using System.Windows;

namespace NetWorthTracker.Entry;

/// <summary>
/// Interaction logic for EntryWindow.xaml
/// </summary>
public partial class EntryWindow : Window
{
    public EntryWindow(IEntryWindowViewModel entryWindowViewModel, User user)
    {
        InitializeComponent();
        DataContext = entryWindowViewModel;
        entryWindowViewModel.User = user;
        entryWindowViewModel.LoadData();
    }
}
