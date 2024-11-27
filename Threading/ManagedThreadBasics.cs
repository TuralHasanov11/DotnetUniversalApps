namespace Threading;

public class ManagedThreadBasics
{
    // Thread-static fields to store data specific to each thread
    [ThreadStatic]
    private static double Previous = 0.0;

    [ThreadStatic]
    private static double Sum = 0.0;

    [ThreadStatic]
    private static int Calls = 0;

    [ThreadStatic]
    private static bool Abnormal;

    // Static fields shared among all threads
    private static int TotalNumber = 0;

    private static CountdownEvent CountdownEvent;

    private static object LockObject = new();

    private static Random Random = new();

    // Constructor to initialize CountdownEvent and LockObject
    public ManagedThreadBasics()
    {
        CountdownEvent = new CountdownEvent(1); // Initialize with a count of 1 for the main thread
        LockObject = new();
    }

    // Entry point to run the example
    public static void Run()
    {
        ManagedThreadBasics example = new();
        Thread.CurrentThread.Name = "Main"; // Name the main thread
        example.Execute(); // Start the execution
        CountdownEvent.Wait(); // Wait for all threads to complete
        Console.WriteLine("{0:N0} random numbers were generated.", TotalNumber); // Print the total number of random numbers generated
    }

    // Method to create and start worker threads
    private void Execute()
    {
        for (int threads = 0; threads < 10; threads++)
        {
            Thread thread = new(new ThreadStart(GetRandomNumbers));
            CountdownEvent.AddCount(); // Increment the count for each new thread
            thread.Name = threads.ToString(); // Name the thread
            thread.Start(); // Start the thread
        }
        GetRandomNumbers(); // Also run the method in the main thread
    }

    // Method to generate random numbers
    private void GetRandomNumbers()
    {
        double result = 0.0;

        for (int ctr = 0; ctr < 2000000; ctr++)
        {
            lock (LockObject) // Ensure thread-safe access to shared resources
            {
                result = Random.NextDouble(); // Generate a random number
                Calls++; // Increment the call count for the current thread
                Interlocked.Increment(ref TotalNumber); // Atomically increment the total number of random numbers generated

                // Check if the same random number is generated twice
                if (result == Previous)
                {
                    Abnormal = true; // Set abnormal flag if the same number is generated
                    break; // Exit the loop
                }
                else
                {
                    Previous = result; // Update the previous number
                    Sum += result; // Add the result to the sum
                }
            }
        }

        // If an abnormal condition was detected, print the result
        if (Abnormal)
            Console.WriteLine("Result is {0} in {1}", Previous, Thread.CurrentThread.Name);

        // Print the summary for the current thread
        Console.WriteLine("Thread {0} finished random number generation.", Thread.CurrentThread.Name);
        Console.WriteLine("Sum = {0:N4}, Mean = {1:N4}, n = {2:N0}\n", Sum, Sum / Calls, Calls);

        CountdownEvent.Signal(); // Signal that the current thread has finished
    }
}