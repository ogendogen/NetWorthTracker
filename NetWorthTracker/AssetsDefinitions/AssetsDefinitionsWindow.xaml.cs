using NetWorthTracker.Database.Models;
using NetWorthTracker.Database.Repositories.Interfaces;
using System.Windows;

namespace NetWorthTracker.AssetsDefinitions;

/// <summary>
/// Interaction logic for AssetsDefinitionsWindow.xaml
/// </summary>
public partial class AssetsDefinitionsWindow : Window
{
    public AssetsDefinitionsWindow(IDefinitionsViewModel assetsDefinitionsViewModel, User user, DefinitionType definitionType) // todo: inject user and parameter which definitions collection to fetch
    {
        InitializeComponent();
        DataContext = assetsDefinitionsViewModel;
        assetsDefinitionsViewModel.User = user;
        assetsDefinitionsViewModel.LoadDefinitions(definitionType);
        Closed += assetsDefinitionsViewModel.OnClosed;
    }
}