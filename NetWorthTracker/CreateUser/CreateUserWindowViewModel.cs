using NetWorthTracker.Database.Repositories.Interfaces;
using System.ComponentModel;
using System.Windows.Input;
using NetWorthTracker.Database.Models;
using System.Windows;
using NetWorthTracker.RelayCommands;

namespace NetWorthTracker.CreateUser;
public interface ICreateNewWindowViewModel
{
    ICommand CreateUser { get; }
    Action CloseWindow { get; set; }
    event EventHandler UserCreated;
}

public class CreateUserWindowViewModel : ICreateNewWindowViewModel, INotifyPropertyChanged
{
    private string _userName;
    public string UserName
    {
        get => _userName;
        set
        {
            _userName = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UserName)));
        }
    }

    private bool _isAddDefaultDefinitions = true;
    public bool IsAddDefaultDefinitions
    {
        get => _isAddDefaultDefinitions;
        set
        {
            _isAddDefaultDefinitions = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsAddDefaultDefinitions)));
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    public event EventHandler UserCreated;

    public Action CloseWindow { get; set; }

    public CreateUserWindowViewModel(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    private ICommand _createUser;
    private readonly IUserRepository _userRepository;

    public ICommand CreateUser => _createUser ??= new AsyncRelayCommand(ExecuteCreateUserButton);

    private async Task ExecuteCreateUserButton()
    {
        var result = await _userRepository.CreateUser(new User() { Name = UserName }, _isAddDefaultDefinitions);
        if (result.IsFailed)
        {
            MessageBox.Show(result.Errors.First().Message, "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        MessageBox.Show("Użytkownik stworzony", "Informacja", MessageBoxButton.OK, MessageBoxImage.Information);
        UserCreated?.Invoke(this, EventArgs.Empty);
        CloseWindow?.Invoke();
    }
}