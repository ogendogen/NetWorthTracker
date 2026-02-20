using NetWorthTracker.Database.Models;
using System.Windows;

namespace NetWorthTracker.Entry;

/// <summary>
/// Interaction logic for EntryWindow.xaml
/// </summary>
public partial class EntryWindow : Window
{
    public EntryWindow(IEntryWindowViewModel entryWindowViewModel, User user, WindowMode windowMode, Database.Models.Entry? entry = null)
    {
        InitializeComponent();
        DataContext = entryWindowViewModel;
        entryWindowViewModel.User = user;
        entryWindowViewModel.Entry = entry!;
        entryWindowViewModel.WindowMode = windowMode;
        entryWindowViewModel.LoadData();
    }

    private void AssetDataGrid_LostFocus(object sender, RoutedEventArgs e)
    {
        if (DataContext is IEntryWindowViewModel vm)
        {
            vm.OnAssetsDataGridLostFocus();
        }
    }

    private void DebtDataGrid_LostFocus(object sender, RoutedEventArgs e)
    {
        if (DataContext is IEntryWindowViewModel vm)
        {
            vm.OnDebtsDataGridLostFocus();
        }
    }
}
