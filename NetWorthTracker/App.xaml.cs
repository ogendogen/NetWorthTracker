using Microsoft.Extensions.DependencyInjection;
using NetWorthTracker.AssetsDefinitions;
using NetWorthTracker.CreateUser;
using NetWorthTracker.Database;
using NetWorthTracker.Database.Repositories;
using NetWorthTracker.Database.Repositories.Interfaces;
using NetWorthTracker.Login;
using NetWorthTracker.Main;
using System.Configuration;
using System.Data;
using System.Windows;

namespace NetWorthTracker;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private IServiceProvider _serviceProvider;

    protected override void OnStartup(StartupEventArgs e)
    {
        var serviceCollection = new ServiceCollection();
        ConfigureServices(serviceCollection);

        _serviceProvider = serviceCollection.BuildServiceProvider();

        var mainWindow = _serviceProvider.GetRequiredService<LoginWindow>();
        mainWindow.Show();
    }

    private void ConfigureServices(IServiceCollection services)
    {
        // Configure Logging
        services.AddLogging();

        // Register Services
        //services.AddSingleton<IUserService, UserService>();
        services.AddDbContext<NetWorthTrackerDbContext>();
        services.AddSingleton<IUserRepository, UserRepository>();
        services.AddSingleton<IEntryRepository, EntryRepository>();
        services.AddSingleton<IAssetRepository, AssetRepository>();
        services.AddSingleton<IDebtRepository, DebtRepository>();
        services.AddSingleton<IAssetDefinitionRepository, AssetDefinitionRepository>();
        services.AddSingleton<IDebtDefinitionRepository, DebtDefinitionRepository>();

        // Register ViewModels
        services.AddSingleton<ILoginViewModel, LoginViewModel>();
        services.AddSingleton<ICreateNewWindowViewModel, CreateUserWindowViewModel>();
        services.AddSingleton<IMainWindowViewModel, MainWindowViewModel>();
        services.AddSingleton<IAssetsDefinitionsViewModel, AssetsDefinitionsViewModel>();

        // Register Views
        services.AddSingleton<LoginWindow>();
    }

    private void OnExit(object sender, ExitEventArgs e)
    {
        // Dispose of services if needed
        if (_serviceProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
}
