namespace SyncClock.Core.Data;

public struct SingleSync
{
    public SingleSync(bool success, DateTime syncDateTime)
    {
        WasCompleted = success;
        Conclusion = syncDateTime;
    }

    public bool WasCompleted { get; set; }
    public DateTime Conclusion { get; set; }
}