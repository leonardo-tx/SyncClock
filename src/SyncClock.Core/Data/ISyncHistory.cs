namespace SyncClock.Core.Data;

public interface ISyncHistory
{
    public int Count { get; }
    public SingleSync this[int index] { get; }

    public void Add(SingleSync item);
    public IEnumerable<SingleSync> GetAll();
}