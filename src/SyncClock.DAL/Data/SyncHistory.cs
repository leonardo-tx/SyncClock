using SyncClock.Core.Data;

namespace SyncClock.DAL.Data;

public sealed class SyncHistory : ISyncHistory
{
    private List<SingleSync> History { get; } = new List<SingleSync>(10);

    public SingleSync this[int index] => History[index];
    public int Count => History.Count;

    public void Add(SingleSync item)
    {
        if (History.Count >= History.Capacity)
        {
            History.RemoveAt(0);
        }
        History.Add(item);
    }

    public IEnumerable<SingleSync> GetAll()
    {
        return History;
    }
}