using GuerrillaNtp;
using Microsoft.Extensions.Options;
using SyncClock.Core.Settings;
using SyncClock.Core.Time;
using SyncClock.Core.Utilities;
using System.Runtime.InteropServices;

namespace SyncClock.DAL.Helpers;

public sealed class SyncHelper : ISyncHelper
{
    public SyncHelper(IOptions<SyncConfiguration> configuration)
    {
        Configuration = configuration.Value;
    }

    public SyncConfiguration Configuration { get; }

    public async Task<bool> ChangeSystemTime()
    {
        try
        {
            var accurateTime = await GetCorrectDateTime();
            var systemTime = new SystemTime(accurateTime);
            return SetSystemTime(ref systemTime);
        }
        catch (InvalidOperationException)
        {
            return false;
        }
    }

    public async Task<double> GetDelayFromNow()
    {
        try
        {
            return Math.Round((await GetCorrectDateTime() - DateTime.UtcNow).TotalSeconds, 3);
        }
        catch (InvalidOperationException)
        {
            return double.NaN;
        }
    }

    private async Task<DateTime> GetCorrectDateTime()
    {
        foreach (var url in Configuration.NtpServers)
        {
            var client = new NtpClient(url);
            try
            {
                var clock = await client.QueryAsync();
                return DateTime.UtcNow + clock.CorrectionOffset;
            }
            catch
            {
                // Do nothing
            }
        }
        throw new InvalidOperationException();
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool SetSystemTime(ref SystemTime system);
}