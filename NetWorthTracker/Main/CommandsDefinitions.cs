using NetWorthTracker.RelayCommands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace NetWorthTracker.Main;

public partial class MainWindowViewModel
{
    private ICommand _actives;
    public ICommand Actives => _actives ??= new RelayCommand(ExecuteActivesCommand);

    private ICommand _debts;
    public ICommand Debts => _debts ??= new RelayCommand(ExecuteDebtsCommand);

    private ICommand _addEntry;
    public ICommand AddEntry => _addEntry ??= new RelayCommand(ExecuteAddEntryCommand);

    private ICommand _removeEntry;
    public ICommand RemoveEntry => _removeEntry ??= new RelayCommand(ExecuteRemoveEntryCommand);
}
