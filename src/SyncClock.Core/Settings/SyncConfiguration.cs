using System.ComponentModel.DataAnnotations;

namespace SyncClock.Core.Settings;

public sealed class SyncConfiguration
{
    [Required]
    public IEnumerable<string> NtpServers { get; set; } = null!;

    [Required]
    [Range(60000, int.MaxValue)]
    public int SyncTime { get; set; } = 3600000;
}