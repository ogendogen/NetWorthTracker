using NetWorthTracker.Database.Models;
using System.Windows;

namespace NetWorthTracker.AssetsDefinitions;

/// <summary>
/// Interaction logic for AssetsDefinitionsWindow.xaml
/// </summary>
public partial class AssetsDefinitionsWindow : Window
{
    public AssetsDefinitionsWindow(IDefinitionsViewModel assetsDefinitionsViewModel, User user, DefinitionType definitionType)
    {
        InitializeComponent();
        DataContext = assetsDefinitionsViewModel;
        assetsDefinitionsViewModel.User = user;
        assetsDefinitionsViewModel.LoadDefinitions(definitionType);
        Closed += assetsDefinitionsViewModel.OnClosed;
    }
}