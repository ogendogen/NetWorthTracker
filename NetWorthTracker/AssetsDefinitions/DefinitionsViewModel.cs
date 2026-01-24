using NetWorthTracker.Database.Models;
using NetWorthTracker.Database.Repositories.Interfaces;
using System.Collections.ObjectModel;
using System.Windows;

namespace NetWorthTracker.AssetsDefinitions;

public interface IDefinitionsViewModel
{
    User User { get; set; }
    void LoadDefinitions(DefinitionType definitionType);
    void OnClosed(object sender, EventArgs e);
}

public class DefinitionsViewModel : IDefinitionsViewModel
{
    public ObservableCollection<Definition> Definitions { get; set; } = new();
	private User _user;
    private readonly IDefinitionRepository _definitionRepository;

    public event Action CloseRequested;

    public User User
	{
		get { return _user; }
		set { _user = value; }
	}

    public DefinitionsViewModel(IDefinitionRepository assetDefinitionRepository)
    {
        _definitionRepository = assetDefinitionRepository;
    }

    public async void OnClosed(object sender, EventArgs e)
    {
        await _definitionRepository.SyncUserDefinitions(_user.Id, Definitions);
    }

    public async void LoadDefinitions(DefinitionType definitionType)
    {
        Definitions.Clear();

        var definitions = await _definitionRepository.GetDefinitionsByUserId(User.Id, definitionType);
        if (definitions.IsFailed)
        {
            MessageBox.Show("Błąd odczytu definicji", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        foreach (var assetDefinition in definitions.Value)
        {
            Definitions.Add(assetDefinition);
        }
    }
}
