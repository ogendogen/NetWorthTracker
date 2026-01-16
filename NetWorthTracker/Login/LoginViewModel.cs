using Microsoft.Extensions.Logging;
using NetWorthTracker.Database.Models;
using NetWorthTracker.Database.Repositories.Interfaces;
using System.Collections.ObjectModel;
using System.Windows.Input;
using NetWorthTracker.CreateUser;
using NetWorthTracker.RelayCommands;

namespace NetWorthTracker.Login;

public interface ILoginViewModel
{
    ICommand Login { get; }
    ICommand OpenUserCreationWindow { get; }
}

public class LoginViewModel : ILoginViewModel
{
    private readonly ILogger<LoginViewModel> _logger;
    private readonly IUserRepository _userRepository;
    private readonly ICreateNewWindowViewModel _createNewWindowViewModel;

    public ObservableCollection<User> Users { get; } = new();

    public LoginViewModel(ILogger<LoginViewModel> logger, IUserRepository userRepository, ICreateNewWindowViewModel createNewWindowViewModel)
    {
        _logger = logger;
        _userRepository = userRepository;
        _createNewWindowViewModel = createNewWindowViewModel;
        _createNewWindowViewModel.UserCreated += OnUserCreated;
        LoadUsers();
    }

    private void OnUserCreated(object sender, EventArgs e)
    {
        LoadUsers();
    }

    public async void LoadUsers()
    {
        Users.Clear();
        var usersResult = await _userRepository.GetAllUsers();
        foreach (var user in usersResult.Value)
        {
            Users.Add(user);
        }
    }

    public ICommand Login => throw new NotImplementedException();

    private ICommand _openUserCreationWindow;

    public ICommand OpenUserCreationWindow =>
        _openUserCreationWindow ??= new RelayCommand(ExecuteOpenUserCreationWindow);

    private void ExecuteOpenUserCreationWindow()
    {
        var createUserWindow = new CreateUser.CreateUserWindow(_createNewWindowViewModel);
        createUserWindow.ShowDialog();
    }
}
