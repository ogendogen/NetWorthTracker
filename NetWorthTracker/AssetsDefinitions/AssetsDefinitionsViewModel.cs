using NetWorthTracker.Database.Models;
using NetWorthTracker.Database.Repositories.Interfaces;
using System.Collections.ObjectModel;
using System.Windows;

namespace NetWorthTracker.AssetsDefinitions;

public interface IAssetsDefinitionsViewModel
{
    User User { get; set; }
    void LoadAssetDefinitions();
    void OnClosed(object sender, EventArgs e);
}

public class AssetsDefinitionsViewModel : IAssetsDefinitionsViewModel
{
    public ObservableCollection<AssetDefinition> Definitions { get; set; } = new();
	private User _user;
    private readonly IAssetDefinitionRepository _assetDefinitionRepository;

    public event Action CloseRequested;

    public User User
	{
		get { return _user; }
		set { _user = value; }
	}

    public AssetsDefinitionsViewModel(IAssetDefinitionRepository assetDefinitionRepository)
    {
        _assetDefinitionRepository = assetDefinitionRepository;
    }

    public async void OnClosed(object sender, EventArgs e)
    {
        await _assetDefinitionRepository.SyncUserDefinitions(_user.Id, Definitions);
    }

    public async void LoadAssetDefinitions()
    {
        Definitions.Clear();

        var assetDefinitions = await _assetDefinitionRepository.GetByUserId(User.Id);
        if (assetDefinitions.IsFailed)
        {
            MessageBox.Show("Błąd odczytu definicji", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        foreach (var assetDefinition in assetDefinitions.Value)
        {
            Definitions.Add(assetDefinition);
        }
    }
}
