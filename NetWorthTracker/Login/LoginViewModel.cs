using Microsoft.Extensions.Logging;
using NetWorthTracker.Database.Models;
using NetWorthTracker.Database.Repositories.Interfaces;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using NetWorthTracker.CreateUser;
using NetWorthTracker.RelayCommands;
using NetWorthTracker.Main;

namespace NetWorthTracker.Login;

public interface ILoginViewModel
{
    ICommand Login { get; }
    ICommand OpenUserCreationWindow { get; }
    event Action CloseRequested;
}

public class LoginViewModel : ILoginViewModel, INotifyPropertyChanged
{
    private readonly ILogger<LoginViewModel> _logger;
    private readonly IUserRepository _userRepository;
    private readonly ICreateNewWindowViewModel _createNewWindowViewModel;
    private readonly IMainWindowViewModel _mainWindowViewModel;
    private User _selectedUser;
    private bool _isAddDefaultDefinitions;

    public event Action CloseRequested;

    public ObservableCollection<User> Users { get; } = new();

    public bool IsAddDefaultDefinitions
    {
        get => _isAddDefaultDefinitions;
        set
        {
            _isAddDefaultDefinitions = value;
            OnPropertyChanged();
        }
    }

    public User SelectedUser
    {
        get => _selectedUser;
        set
        {
            _selectedUser = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public LoginViewModel(ILogger<LoginViewModel> logger, IUserRepository userRepository, ICreateNewWindowViewModel createNewWindowViewModel, IMainWindowViewModel mainWindowViewModel)
    {
        _logger = logger;
        _userRepository = userRepository;
        _createNewWindowViewModel = createNewWindowViewModel;
        _mainWindowViewModel = mainWindowViewModel;
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

    private ICommand _login;
    public ICommand Login =>
        _login ??= new RelayCommand(ExecuteLogin, () => SelectedUser != null);

    private void ExecuteLogin()
    {
        MainWindow mainWindow = new MainWindow(_mainWindowViewModel, SelectedUser);
        mainWindow.Show();
        CloseRequested?.Invoke();
    }

    private ICommand _openUserCreationWindow;

    public ICommand OpenUserCreationWindow =>
        _openUserCreationWindow ??= new RelayCommand(ExecuteOpenUserCreationWindow);

    private void ExecuteOpenUserCreationWindow()
    {
        var createUserWindow = new CreateUserWindow(_createNewWindowViewModel);
        createUserWindow.ShowDialog();
    }
}
