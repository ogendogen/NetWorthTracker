using FluentResults;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using NetWorthTracker.Database.Models;
using NetWorthTracker.Database.Repositories;
using NetWorthTracker.RelayCommands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace NetWorthTracker.Entry;

public interface IEntryWindowViewModel
{
    void LoadData();
    User User { get; set; }
    Database.Models.Entry Entry { get; set; }
    WindowMode WindowMode { get; set; }
}

public class EntryWindowViewModel : IEntryWindowViewModel, INotifyPropertyChanged
{
    private readonly IAssetRepository _assetRepository;
    private readonly IDebtRepository _debtRepository;
    private readonly IDefinitionRepository _definitionRepository;
    private readonly IEntryRepository _entryRepository;
    private ObservableCollection<Asset> _assets;
    public ObservableCollection<Asset> Assets
    {
        get => _assets;
        set
        {
            if (_assets != null)
                _assets.CollectionChanged -= OnAssetsChanged;
            _assets = value;
            if (_assets != null)
                _assets.CollectionChanged += OnAssetsChanged;
            OnPropertyChanged(nameof(Assets));
            OnPropertyChanged(nameof(AssetsSum));
            OnPropertyChanged(nameof(TotalSumFormatted));
        }
    }

    private ObservableCollection<Debt> _debts;
    public ObservableCollection<Debt> Debts
    {
        get => _debts;
        set
        {
            if (_debts != null)
                _debts.CollectionChanged -= OnDebtsChanged;
            _debts = value;
            if (_debts != null)
                _debts.CollectionChanged += OnDebtsChanged;
            OnPropertyChanged(nameof(Debts));
            OnPropertyChanged(nameof(DebtsSum));
            OnPropertyChanged(nameof(TotalSumFormatted));
        }
    }

    private void OnAssetsChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems != null)
            foreach (Asset item in e.OldItems)
                item.PropertyChanged -= OnAssetPropertyChanged;
        if (e.NewItems != null)
            foreach (Asset item in e.NewItems)
                item.PropertyChanged += OnAssetPropertyChanged;
        OnPropertyChanged(nameof(AssetsSum));
        OnPropertyChanged(nameof(TotalSumFormatted));
    }

    private void OnDebtsChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems != null)
            foreach (Debt item in e.OldItems)
                item.PropertyChanged -= OnDebtPropertyChanged;
        if (e.NewItems != null)
            foreach (Debt item in e.NewItems)
                item.PropertyChanged += OnDebtPropertyChanged;
        OnPropertyChanged(nameof(DebtsSum));
        OnPropertyChanged(nameof(TotalSumFormatted));
    }

    private void OnAssetPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Asset.Value))
        {
            OnPropertyChanged(nameof(AssetsSum));
            OnPropertyChanged(nameof(TotalSumFormatted));
        }
    }

    private void OnDebtPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Debt.Value))
        {
            OnPropertyChanged(nameof(DebtsSum));
            OnPropertyChanged(nameof(TotalSumFormatted));
        }
    }

    public string AssetsSum => $"Suma: {Assets?.Sum(x => x.Value).ToString("N2") ?? "0.00"} zł";
    public string DebtsSum => $"Suma: {Debts?.Sum(x => x.Value).ToString("N2") ?? "0.00"} zł";
    public decimal TotalSum => (Assets?.Sum(x => x.Value) ?? 0) - (Debts?.Sum(x => x.Value) ?? 0);
    public string TotalSumFormatted => $"Całkowita suma: {(TotalSum):N2} zł";
    public User User { get; set; }
    public Database.Models.Entry Entry { get; set; }
    public WindowMode WindowMode { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    public EntryWindowViewModel(IDefinitionRepository definitionRepository, IEntryRepository entryRepository)
    {
        _definitionRepository = definitionRepository;
        _entryRepository = entryRepository;
    }

    public async void LoadData()
    {
        // todo: handling edit (Entry is not null)
        var assetsDefinitions = await _definitionRepository.GetDefinitionsByUserId(User.Id, DefinitionType.Asset);
        var assetsList = assetsDefinitions.Value.Select(assetDefinition => new Asset()
        {
            Name = assetDefinition.Name,
        }).ToList();
        
        Assets = new ObservableCollection<Asset>(assetsList);
        foreach (var asset in Assets)
            asset.PropertyChanged += OnAssetPropertyChanged;

        var debtsDefinitions = await _definitionRepository.GetDefinitionsByUserId(User.Id, DefinitionType.Debt);
        var debtsList = debtsDefinitions.Value.Select(debtDefinition => new Debt()
        {
            Name = debtDefinition.Name,
        }).ToList();
        
        Debts = new ObservableCollection<Debt>(debtsList);
        foreach (var debt in Debts)
            debt.PropertyChanged += OnDebtPropertyChanged;
    }

    private ICommand _saveEntryCommand;
    public ICommand SaveEntryCommand =>
        _saveEntryCommand ??= new RelayCommand(ExecuteSaveEntry);

    private async void ExecuteSaveEntry(object obj)
    {
        if (WindowMode == WindowMode.Create)
        {
            var result = await _entryRepository.AddEntry(new Database.Models.Entry()
            {
                Assets = Assets.Where(x => x.Value != 0).ToList(),
                Debts = Debts.Where(x => x.Value != 0).ToList(),
                Date = DateTime.Now,
                UserId = User.Id,
                User = User,
                Value = TotalSum,
            });

            if (result.IsSuccess)
            {
                MessageBox.Show("Zapisano pomyślnie!", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
                WindowMode = WindowMode.Edit;
                Entry = result.Value;
            }
            else
            {
                MessageBox.Show($"Nie udało się zapisać! {result.Errors[0].Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        else if (WindowMode == WindowMode.Edit)
        {
            var result = await _entryRepository.UpdateEntry(Entry);
            if (result.IsSuccess)
            {
                MessageBox.Show("Zapisano pomyślnie!", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show($"Nie udało się zapisać! {result.Errors[0].Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    public ISeries[] Series { get; set; } =
    {
        new LineSeries<double>
        {
            Values = new double[] { 2, 5, 4, 6, 8 }
        }
    };

}
