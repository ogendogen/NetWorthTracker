using System.Windows;

namespace NetWorthTracker.CreateUser;

/// <summary>
/// Interaction logic for CreateUserWindow.xaml
/// </summary>
public partial class CreateUserWindow : Window
{
    public CreateUserWindow(ICreateNewWindowViewModel createNewWindowViewModel)
    {
        InitializeComponent();
        DataContext = createNewWindowViewModel;
        createNewWindowViewModel.CloseWindow = Close;
    }
}
