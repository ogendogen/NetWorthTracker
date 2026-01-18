using NetWorthTracker.AssetsDefinitions;
using NetWorthTracker.Database.Models;
using NetWorthTracker.Definitions;
using NetWorthTracker.RelayCommands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace NetWorthTracker.Main;

public interface IMainWindowViewModel
{
    ICommand Actives { get; }
    ICommand Debts { get; }
    User User { get; set; }
}

public class MainWindowViewModel : IMainWindowViewModel
{
    private ICommand _actives;
    public ICommand Actives => _actives ??= new RelayCommand(ExecuteActivesCommand);

    private readonly IAssetsDefinitionsViewModel _assetsDefinitionsViewModel;
    private User _user;

    public User User
    {
        get { return _user; }
        set { _user = value; }
    }

    public MainWindowViewModel(IAssetsDefinitionsViewModel assetsDefinitionsViewModel)
    {
        _assetsDefinitionsViewModel = assetsDefinitionsViewModel;
    }

    private void ExecuteActivesCommand(object obj)
    {
        AssetsDefinitionsWindow assetsDefinitionsWindow = new AssetsDefinitionsWindow(_assetsDefinitionsViewModel, _user);
        assetsDefinitionsWindow.ShowDialog();
    }

    private ICommand _debts;

    public ICommand Debts => _debts ??= new RelayCommand(ExecuteDebtsCommand);

    private void ExecuteDebtsCommand(object obj)
    {
        //DefinitionsWindow definitionsWindow = new DefinitionsWindow();
        //definitionsWindow.ShowDialog();
    }
}
