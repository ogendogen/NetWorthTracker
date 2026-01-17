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
}

public class MainWindowViewModel : IMainWindowViewModel
{
    private ICommand _actives;
    public ICommand Actives => _actives ??= new RelayCommand(ExecuteActivesCommand);

    private void ExecuteActivesCommand(object obj)
    {
        DefinitionsWindow definitionsWindow = new DefinitionsWindow();
        definitionsWindow.ShowDialog();
    }

    private ICommand _debts;
    public ICommand Debts => _debts ??= new RelayCommand(ExecuteDebtsCommand);

    private void ExecuteDebtsCommand(object obj)
    {
        DefinitionsWindow definitionsWindow = new DefinitionsWindow();
        definitionsWindow.ShowDialog();
    }
}
