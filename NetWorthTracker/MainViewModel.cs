using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace NetWorthTracker;

public interface IMainViewModel
{
    ICommand SaveCommand { get; }
}

public class MainViewModel : IMainViewModel
{
    private readonly ILogger<MainViewModel> _logger;

    public MainViewModel(ILogger<MainViewModel> logger)
    {
        _logger = logger;
    }

    public ICommand SaveCommand => throw new NotImplementedException();

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
