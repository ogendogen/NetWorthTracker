using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using NetWorthTracker.Database.Models;
using NetWorthTracker.Database.Repositories;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;

namespace NetWorthTracker.Entry;

public interface IEntryWindowViewModel
{
    void LoadData();
    User User { get; set; }
}

public class EntryWindowViewModel : IEntryWindowViewModel, INotifyPropertyChanged
{
    private readonly IAssetRepository _assetRepository;
    private readonly IDebtRepository _debtRepository;
    private readonly IDefinitionRepository _definitionRepository;

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
            OnPropertyChanged(nameof(TotalSum));
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
            OnPropertyChanged(nameof(TotalSum));
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
        OnPropertyChanged(nameof(TotalSum));
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
        OnPropertyChanged(nameof(TotalSum));
    }

    private void OnAssetPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Asset.Value))
        {
            OnPropertyChanged(nameof(AssetsSum));
            OnPropertyChanged(nameof(TotalSum));
        }
    }

    private void OnDebtPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Debt.Value))
        {
            OnPropertyChanged(nameof(DebtsSum));
            OnPropertyChanged(nameof(TotalSum));
        }
    }

    public Database.Models.Entry SelectedEntry { get; set; }
    public string AssetsSum => $"Suma: {Assets?.Sum(x => x.Value).ToString("N2") ?? "0.00"} zł";
    public string DebtsSum => $"Suma: {Debts?.Sum(x => x.Value).ToString("N2") ?? "0.00"} zł";
    public string TotalSum => $"Całkowita suma: {(Assets?.Sum(x => x.Value) ?? 0) - (Debts?.Sum(x => x.Value) ?? 0):N2} zł";
    public User User { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    public EntryWindowViewModel(IDefinitionRepository definitionRepository)
    {
        _definitionRepository = definitionRepository;
    }

    public async void LoadData()
    {
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

    public ISeries[] Series { get; set; } =
    {
        new LineSeries<double>
        {
            Values = new double[] { 2, 5, 4, 6, 8 }
        }
    };

}
