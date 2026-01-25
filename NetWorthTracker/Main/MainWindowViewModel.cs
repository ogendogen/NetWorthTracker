using NetWorthTracker.AssetsDefinitions;
using NetWorthTracker.Database.Models;
using NetWorthTracker.Database.Repositories;
using NetWorthTracker.Entry;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace NetWorthTracker.Main;

public interface IMainWindowViewModel
{
    ICommand Actives { get; }
    ICommand Debts { get; }
    ICommand AddEntry { get; }
    ICommand RemoveEntry { get; }
    User User { get; set; }
}

public partial class MainWindowViewModel : IMainWindowViewModel, INotifyPropertyChanged
{
    private readonly IDefinitionsViewModel _assetsDefinitionsViewModel;
    private readonly IEntryRepository _entryRepository;
    private readonly IEntryWindowViewModel _entryWindowViewModel;
    private Database.Models.Entry _selectedEntry;

    public Database.Models.Entry SelectedEntry
    {
        get => _selectedEntry;
        set
        {
            _selectedEntry = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<Database.Models.Entry> Entries { get; private set; } = new();

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public User User { get; set; }

    public MainWindowViewModel(IDefinitionsViewModel assetsDefinitionsViewModel, IEntryRepository entryRepository, IEntryWindowViewModel entryWindowViewModel)
    {
        _assetsDefinitionsViewModel = assetsDefinitionsViewModel;
        _entryRepository = entryRepository;
        _entryWindowViewModel = entryWindowViewModel;
    }

    public async void LoadEntries()
    {
        var entries = await _entryRepository.GetUserEntries(User.Id);
        if (entries.IsSuccess)
        {
            Entries = new ObservableCollection<Database.Models.Entry>(entries.Value);
        }
    }

    private void ExecuteActivesCommand(object obj)
    {
        AssetsDefinitionsWindow assetsDefinitionsWindow = new AssetsDefinitionsWindow(_assetsDefinitionsViewModel, User, DefinitionType.Asset);
        assetsDefinitionsWindow.ShowDialog();
    }

    private void ExecuteAddEntryCommand(object obj)
    {
        EntryWindow entryWindow = new EntryWindow(_entryWindowViewModel);
        entryWindow.ShowDialog();
    }

    private async void ExecuteRemoveEntryCommand(object obj)
    {
        if (SelectedEntry == null)
        {
            MessageBox.Show("Wybierz wpis do usunięcia", "Informacja", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var result = MessageBox.Show("Czy na pewno chcesz usunąć wybrany wpis? Ta operacja jest nieodwracalna i usunie powiązane wpisy!", "Uwaga", MessageBoxButton.YesNo, MessageBoxImage.Warning);
        if (result == MessageBoxResult.Yes)
        {
            await _entryRepository.RemoveEntry(SelectedEntry.Id);
        }
    }

    private void ExecuteDebtsCommand(object obj)
    {
        AssetsDefinitionsWindow assetsDefinitionsWindow = new AssetsDefinitionsWindow(_assetsDefinitionsViewModel, User, DefinitionType.Debt);
        assetsDefinitionsWindow.ShowDialog();
    }
}
