using SyncClock.Core.Data;
using SyncClock.Core.Utilities;

namespace SyncClock.Client.Workers;

public sealed class SyncHandler : BackgroundService
{
    public SyncHandler(ISyncHelper syncHelper, ISyncHistory syncHistory)
    {
        SyncHelper = syncHelper;
        SyncHistory = syncHistory;
    }

    private ISyncHelper SyncHelper { get; }
    private ISyncHistory SyncHistory { get; }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            SyncHistory.Add(new SingleSync(await SyncHelper.ChangeSystemTime(), DateTime.Now));
            await Task.Delay(SyncHelper.Configuration.SyncTime, stoppingToken).ContinueWith(task => { }, CancellationToken.None);
        }
    }
}