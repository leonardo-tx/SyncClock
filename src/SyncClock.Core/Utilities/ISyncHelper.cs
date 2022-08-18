using SyncClock.Core.Settings;

namespace SyncClock.Core.Utilities;

public interface ISyncHelper
{
    public SyncConfiguration Configuration { get; }

    public Task<bool> ChangeSystemTime();
    public Task<double> GetDelayFromNow();
}