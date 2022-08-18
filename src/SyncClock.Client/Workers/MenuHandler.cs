using SyncClock.Client.Resources;
using SyncClock.Core.Data;
using SyncClock.Core.Menu;
using SyncClock.Core.Utilities;
using System.Reflection;

namespace SyncClock.Client.Workers;

public sealed class MenuHandler : BackgroundService
{
    public MenuHandler(IHostApplicationLifetime hostLifetime, ISyncHistory syncHistory, ISyncHelper syncHelper)
    {
        HostLifetime = hostLifetime;
        SyncHistory = syncHistory;
        SyncHelper = syncHelper;
    }

    private IHostApplicationLifetime HostLifetime { get; }
    private ISyncHistory SyncHistory { get; }
    private ISyncHelper SyncHelper { get; }
    private MenuOptions Option { get; set; }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while(!stoppingToken.IsCancellationRequested)
        {
            RunMain(stoppingToken);
            RunSpecificOption(Option);
        }
        return Task.CompletedTask;
    }

    private void RunMain(CancellationToken stoppingToken)
    {
        Console.Clear();
        Console.WriteLine(Strings.MainBanner, Math.Round((double)SyncHelper.Configuration.SyncTime / 60000, 1));
        MoveSelection(typeof(MenuOptions), (int)Option);
        while (!stoppingToken.IsCancellationRequested)
        {
            var key = Console.ReadKey(true).Key;
            if (key == ConsoleKey.Enter) return;

            ExecuteKey(key);
        }
    }

    private void ExecuteKey(ConsoleKey key)
    {
        switch (key)
        {
            case ConsoleKey.UpArrow:
                MoveSelection(typeof(MenuOptions), (int)Option - 1);
                break;
            case ConsoleKey.DownArrow:
                MoveSelection(typeof(MenuOptions), (int)Option + 1);
                break;
        }
    }

    private void MoveSelection(Type enumType, int nextOption)
    {
        if (!Enum.IsDefined(enumType, nextOption))
        {
            return;
        }
        if (Console.CursorVisible)
        {
            Console.CursorVisible = false;
        }
        Console.SetCursorPosition(0, (int)Option + 4);
        Console.Write("  ");
        Console.SetCursorPosition(0, nextOption + 4);
        Console.Write(">>");

        Option = (MenuOptions)nextOption;
    }

    private void RunSpecificOption(MenuOptions option)
    {
        Console.Clear();
        Console.CursorVisible = true;
        GetType()
            .GetMethod($"Run{Enum.GetName(typeof(MenuOptions), option)}Menu", BindingFlags.NonPublic | BindingFlags.Instance)!
            .Invoke(this, null);
    }

#pragma warning disable IDE0051
    private void RunCompletedSyncsMenu()
    {
        for (int i = 0; i < SyncHistory.Count; i++)
        {
            var currentSync = SyncHistory[i];
            var message = currentSync.WasCompleted ? Strings.SuccessfulSync : Strings.FailedSync;
            
            Console.WriteLine(message, i + 1, currentSync.Conclusion.ToString("G"));
        }
        Console.Write("\n\n" + Strings.PressAnyKey);
        Console.ReadKey(true);
    }

    private void RunDelayMenu()
    {
        var delayResult = SyncHelper.GetDelayFromNow().Result;
        if (delayResult <= 0)
        {
            Console.WriteLine(Strings.LateClock, delayResult * -1);
        }
        else
        {
            Console.WriteLine(Strings.AheadClock, delayResult);
        }
        Console.Write("\n\n" + Strings.PressAnyKey);
        Console.ReadKey(true);
    }

    private void RunExitMenu()
    {
        HostLifetime.StopApplication();
    }
#pragma warning restore IDE0051
}