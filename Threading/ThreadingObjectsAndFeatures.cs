using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Threading;

public class ThreadingObjectsAndFeatures
{
    public static void Run()
    {
        var timerState = new TimerState { Counter = 0 };

        var timer = new Timer(
                callback: new TimerCallback((object? ts) =>
                {
                    Console.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: starting a new callback.");
                    var state = (TimerState?)ts;
                    if (state is not null)
                    {
                        Interlocked.Increment(ref state.Counter);
                    }
                }),
                state: timerState,
                dueTime: 1000,
                period: 1000
            );

        while (timerState.Counter <= 10)
        {
            Task.Delay(1000).Wait();
        }

        timer.Dispose();
        Console.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: done.");
    }
}

public class TimerState
{
    public int Counter;
}