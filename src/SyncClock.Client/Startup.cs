using SyncClock.Client.Resources;
using SyncClock.Client.Workers;
using SyncClock.Core.Data;
using SyncClock.Core.Settings;
using SyncClock.Core.Utilities;
using SyncClock.DAL.Data;
using SyncClock.DAL.Helpers;
using System.ComponentModel.DataAnnotations;

namespace SyncClock.Client;

public sealed class Startup
{
    private static readonly string _debugBasePath = "Properties\\Settings\\";
    private static readonly string _releaseBasePath = "SyncClock\\";
    private static readonly string _appSettings = "appsettings.json";
    private static readonly string _syncSettings = "syncsettings.json";

    private static string BasePath 
    {
        get
        {
#if DEBUG
            return _debugBasePath;
#else
            return _releaseBasePath;
#endif
        }
    }

    public void ConfigureBuilder(IHostBuilder builder)
    {
        CheckFiles();
        builder.ConfigureServices((hostContext, services) =>
        {
            var configuration = hostContext.Configuration;
            var syncConfiguration = configuration.GetSection("SyncConfiguration");

            if (!syncConfiguration.Exists()) StopApplication();
            ValidateConfiguration(syncConfiguration.Get<SyncConfiguration>());

            services.AddSingleton<ISyncHelper, SyncHelper>();
            services.AddSingleton<ISyncHistory, SyncHistory>();
            
            services.AddHostedService<SyncHandler>();
            services.AddHostedService<MenuHandler>();

            services.Configure<SyncConfiguration>(syncConfiguration);
        });
        builder.ConfigureAppConfiguration((hostContext, configurationBuilder) =>
        {
            configurationBuilder.AddJsonFile(BasePath + _appSettings);
            configurationBuilder.AddJsonFile(BasePath + _syncSettings);
        });
        
    }

    private static void CheckFiles()
    {
        if (!Directory.Exists(BasePath)) Directory.CreateDirectory(BasePath);
        if (!File.Exists(BasePath + _appSettings)) File.WriteAllText(BasePath + _appSettings, Strings.SettingsApp);
        if (!File.Exists(BasePath + _syncSettings)) File.WriteAllText(BasePath + _syncSettings, Strings.SettingsSync);
    }

    private static void ValidateConfiguration(SyncConfiguration syncConfiguration)
    {
        var context = new ValidationContext(syncConfiguration, serviceProvider: null, items: null);
        var validationResults = new List<ValidationResult>();

        if (!Validator.TryValidateObject(syncConfiguration, context, validationResults, true))
        {
            StopApplication();
        }
    }

    private static void StopApplication()
    {
        Console.WriteLine(Strings.InvalidSyncConfiguration);
        Console.Write("\n\n" + Strings.PressAnyKey);

        Console.ReadKey(true);
        Environment.Exit(0);
    }
}