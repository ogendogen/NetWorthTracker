using NetWorthTracker.Database.Models;
using NetWorthTracker.Database.Repositories.Interfaces;
using NetWorthTracker.Definitions;
using System.Windows;

namespace NetWorthTracker.AssetsDefinitions;

/// <summary>
/// Interaction logic for AssetsDefinitionsWindow.xaml
/// </summary>
public partial class AssetsDefinitionsWindow : Window
{
    public AssetsDefinitionsWindow(IAssetsDefinitionsViewModel assetsDefinitionsViewModel, User user) // todo: inject user and parameter which definitions collection to fetch
    {
        InitializeComponent();
        DataContext = assetsDefinitionsViewModel;
        assetsDefinitionsViewModel.User = user;
        assetsDefinitionsViewModel.LoadAssetDefinitions();
        Closed += assetsDefinitionsViewModel.OnClosed;
    }
}