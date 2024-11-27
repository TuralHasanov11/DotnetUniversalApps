namespace Threading;

public class UsingThreads
{
    public static void Run()
    {
        //
        var serverClass = new ServerClass();

        var instanceCaller = new Thread(new ThreadStart(serverClass.InstanceMethod));
        instanceCaller.Name = "InstanceCaller";
        instanceCaller.Start();
        Thread.Sleep(1000);
        instanceCaller.Interrupt();

        Console.WriteLine("The Main() thread calls this after  starting the new InstanceCaller thread.");

        var staticCaller = new Thread(ServerClass.StaticMethod);
        staticCaller.Name = "StaticCaller";
        staticCaller.Priority = ThreadPriority.Lowest;
        staticCaller.Start();

        Console.WriteLine("The Main() thread calls this after starting the new StaticCaller thread.");

        //
        var threadWithState = new ThreadWithState("This report displays the number {0}.", 42, (lineCount) =>
        {
            Console.WriteLine(
            "Independent task printed {0} lines.", lineCount);
        });

        var threadCaller = new Thread(new ThreadStart(threadWithState.ThreadProc));
        threadCaller.Name = "ThreadWithStateCaller";
        threadCaller.Start();
        Console.WriteLine("Main thread does some work, then waits.");
        threadCaller.Join();
        Console.WriteLine("Independent task has completed; main thread ends.");
    }
}

public class ServerClass
{
    public void InstanceMethod()
    {
        try
        {
            Console.WriteLine(
            "ServerClass.InstanceMethod is running on another thread.");

            Thread.Sleep(Timeout.Infinite);

            Console.WriteLine(
                "The instance method called by the worker thread has ended.");
        }
        catch (ThreadInterruptedException)
        {
            Console.WriteLine("Thread '{0}' awoken.",
                              Thread.CurrentThread.Name);
        }
        catch (ThreadAbortException)
        {
            Console.WriteLine("Thread '{0}' aborted.",
                              Thread.CurrentThread.Name);
        }
        finally
        {
            Console.WriteLine("Thread '{0}' executing finally block.",
                              Thread.CurrentThread.Name);
        }
        Console.WriteLine("Thread '{0} finishing normal execution.",
                          Thread.CurrentThread.Name);
        Console.WriteLine();
    }

    public static void StaticMethod()
    {
        Console.WriteLine(
            "ServerClass.StaticMethod is running on another thread.");
        Thread.Sleep(500);
        Console.WriteLine(
            "The static method called by the worker thread has ended.");
    }
}

public class ThreadWithState
{
    // State information used in the task.
    private string boilerplate;

    private int numberValue;

    public Action<int> Callback;

    private static CancellationTokenSource CancellationTokenSource = new();

    // The constructor obtains the state information.
    public ThreadWithState(string text, int number, Action<int> callback)
    {
        boilerplate = text;
        numberValue = number;
        Callback = callback;
    }

    // The thread procedure performs the task, such as formatting
    // and printing a document.
    public void ThreadProc()
    {
        try
        {
            Console.WriteLine(boilerplate, numberValue);
            int x = Random.Shared.Next(2);
            Console.WriteLine("ThreadProc: Random number: {0}", x);
            if (x == 1)
            {
                CancellationTokenSource.Cancel();
                Console.WriteLine("ThreadProc: Task cancellation called");
            }

            CancellationTokenSource.Token.ThrowIfCancellationRequested();

            Callback?.Invoke(1);
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("ThreadProc: Task canceled.");
        }
        finally
        {
            Console.WriteLine("ThreadProc: Thread '{0}' executing finally block.",
                              Thread.CurrentThread.Name);
        }
    }
}