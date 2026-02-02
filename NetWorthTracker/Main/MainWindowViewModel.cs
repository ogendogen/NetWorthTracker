using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using NetWorthTracker.AssetsDefinitions;
using NetWorthTracker.Database.Models;
using NetWorthTracker.Database.Repositories;
using NetWorthTracker.Entry;
using SkiaSharp;
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
    ICommand EditEntry { get; }
    ICommand RemoveEntry { get; }
    User User { get; set; }
    void LoadEntriesAndChartData();
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

    public async void LoadEntriesAndChartData()
    {
        var entries = await _entryRepository.GetUserEntries(User.Id);
        if (entries.IsSuccess)
        {
            Entries.Clear();
            foreach (var entry in entries.Value)
            {
                Entries.Add(entry);
            }

            Series = new ISeries[]
            {
                new LineSeries<DateTimePoint>
                {
                    Values = entries.Value.Select(e => new DateTimePoint(e.Date, (double)e.Value)).ToList()
                }
            };
            
            XAxes = new Axis[]
            {
                new Axis
                {
                    Labeler = value => new DateTime((long)value).ToString("dd/MM/yyyy"),
                    LabelsRotation = 15,
                    // todo: hide steps between
                    //ForceStepToMin = true,
                    //MinStep = TimeSpan.FromDays(1).Ticks
                }
            };
            
            OnPropertyChanged(nameof(Series));
            OnPropertyChanged(nameof(XAxes));
        }
    }

    private void ExecuteActivesCommand(object obj)
    {
        AssetsDefinitionsWindow assetsDefinitionsWindow = new AssetsDefinitionsWindow(_assetsDefinitionsViewModel, User, DefinitionType.Asset);
        assetsDefinitionsWindow.ShowDialog();
    }

    private void ExecuteAddEntryCommand(object obj)
    {
        EntryWindow entryWindow = new EntryWindow(_entryWindowViewModel, User, WindowMode.Create);
        entryWindow.Closed += (s, e) => LoadEntriesAndChartData();
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
            LoadEntriesAndChartData();
        }
    }

    private void ExecuteDebtsCommand(object obj)
    {
        AssetsDefinitionsWindow assetsDefinitionsWindow = new AssetsDefinitionsWindow(_assetsDefinitionsViewModel, User, DefinitionType.Debt);
        assetsDefinitionsWindow.ShowDialog();
    }

    private void ExecuteEditEntryCommand(object obj)
    {
        if (SelectedEntry is not null)
        {
            EntryWindow entryWindow = new EntryWindow(_entryWindowViewModel, User, WindowMode.Edit, SelectedEntry);
            entryWindow.Closed += (s, e) => LoadEntriesAndChartData();
            entryWindow.ShowDialog();
        }
    }

    public ISeries[] Series { get; set; }
    public Axis[] XAxes { get; set; }
}
