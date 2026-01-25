using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using NetWorthTracker.Database.Models;
using NetWorthTracker.Database.Repositories;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace NetWorthTracker.Entry;

public interface IEntryWindowViewModel
{
    void LoadData();
    User User { get; set; }
}

public class EntryWindowViewModel : IEntryWindowViewModel
{
    private readonly IAssetRepository _assetRepository;
    private readonly IDebtRepository _debtRepository;
    private readonly IDefinitionRepository _definitionRepository;

    public ObservableCollection<Asset> Assets { get; set; }
    public ObservableCollection<Debt> Debts { get; set; }
    public Database.Models.Entry SelectedEntry { get; set; }
    public User User { get; set; }
    public EntryWindowViewModel(IDefinitionRepository definitionRepository)
    {
        _definitionRepository = definitionRepository;
    }

    public async void LoadData()
    {
        var assetsDefinitions = await _definitionRepository.GetDefinitionsByUserId(User.Id, DefinitionType.Asset); // todo: userid
        Assets = new ObservableCollection<Asset>(assetsDefinitions.Value.Select(assetDefinition => new Asset()
        {
            Name = assetDefinition.Name,
        }));

        var debtsDefinitions = await _definitionRepository.GetDefinitionsByUserId(User.Id, DefinitionType.Debt); // todo: userid
        Debts = new ObservableCollection<Debt>(debtsDefinitions.Value.Select(debtDefinition => new Debt()
        {
            Name = debtDefinition.Name,
        }));
    }

    public ISeries[] Series { get; set; } =
    {
        new LineSeries<double>
        {
            Values = new double[] { 2, 5, 4, 6, 8 }
        }
    };

}
