using SyncClock.Client;

var builder = Host.CreateDefaultBuilder(args);
var startup = new Startup();

startup.ConfigureBuilder(builder);
var host = builder.Build();

await host.RunAsync();