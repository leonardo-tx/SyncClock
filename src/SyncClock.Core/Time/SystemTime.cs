namespace SyncClock.Core.Time;

public struct SystemTime
{
    public SystemTime(DateTime dateTime)
    {
        Year = (short)dateTime.Year;
        Month = (short)dateTime.Month;
        DayOfWeek = (short)dateTime.DayOfWeek;
        Day = (short)dateTime.Day;
        Hour = (short)dateTime.Hour;
        Minute = (short)dateTime.Minute;
        Second = (short)dateTime.Second;
        Milliseconds = (short)dateTime.Millisecond;
    }

    public short Year { get; }
    public short Month { get; }
    public short DayOfWeek { get; }
    public short Day { get; }
    public short Hour { get; }
    public short Minute { get; }
    public short Second { get; }
    public short Milliseconds { get; }
}