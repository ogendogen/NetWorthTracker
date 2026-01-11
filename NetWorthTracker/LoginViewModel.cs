using Microsoft.Extensions.Logging;
using NetWorthTracker.Database.Models;
using NetWorthTracker.Database.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;

namespace NetWorthTracker;

public interface IMainViewModel
{
    ICommand Login { get; }
    ICommand OpenUserCreationWindow { get; }
}

public class LoginViewModel : IMainViewModel
{
    private readonly ILogger<LoginViewModel> _logger;
    private readonly IUserRepository _userRepository;

    public ObservableCollection<User> Users { get; } = new();

    public LoginViewModel(ILogger<LoginViewModel> logger, IUserRepository userRepository)
    {
        _logger = logger;
        _userRepository = userRepository;
        LoadUsers();
    }

    private async void LoadUsers()
    {
        var users = await _userRepository.GetAllUsers();
        foreach (var user in users)
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
        var createUserWindow = new CreateUserWindow();
        createUserWindow.ShowDialog();
    }

    //private ICommand _saveCommand;
    //public ICommand SaveCommand =>
    //    _saveCommand ??= new AsyncRelayCommand(ExecuteSaveAsync);

    //private async Task ExecuteSaveAsync()
    //{
    //    try
    //    {
    //        _logger.LogInformation("Save command executed");
    //        await _userService.SaveUserDataAsync();
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError(ex, "Error during save operation");
    //    }
    //}
}
