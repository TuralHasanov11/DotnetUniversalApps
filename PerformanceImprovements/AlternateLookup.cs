using System.Buffers;
using System.Diagnostics;
using System.Numerics.Tensors;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace PerformanceImprovements;

public static class AlternateLookup
{
    public static readonly Stopwatch stopwatch = new();

    public static async Task Run()
    {
        await EnumerateMatches();
        await EnumerateSplits();
        await EnumerateSplitsWithCollectionMarshall();
        await SearchValuesExample();
        await SearchValuesExampleWithRegex();
        await TensorPrimitivesExample();
    }

    private static async Task EnumerateMatches()
    {
        Console.WriteLine("EnumerateMatches");
        string text = await File.ReadAllTextAsync(@"D:\Programming\Projects\Roadmap\Dotnet-Universal-Apps\DotnetUniversalApps\PerformanceImprovements\TextFile1.txt");

        Dictionary<string, int> frequency = new(StringComparer.OrdinalIgnoreCase);

        var lookup = frequency.GetAlternateLookup<ReadOnlySpan<char>>();

        for (int j = 0; j < 10; j++)
        {
            long mem = GC.GetTotalAllocatedBytes();
            stopwatch.Restart();

            for (int i = 0; i < 10; i++)
            {
                foreach (var m in Helpers.Words().EnumerateMatches(text))
                {
                    var word = text.AsSpan(m.Index, m.Length);

                    lookup[word] = lookup.TryGetValue(word, out int count) ? count + 1 : 1;
                }
            }

            stopwatch.Stop();
            mem = GC.GetTotalAllocatedBytes() - mem;

            Console.WriteLine($"Time: {stopwatch.Elapsed.TotalMilliseconds}ms, Memory: {mem / 1024.0:N2} mb");
        }
    }

    public static async Task EnumerateSplits()
    {
        Console.WriteLine("EnumerateSplits");
        string text = await File.ReadAllTextAsync(@"D:\Programming\Projects\Roadmap\Dotnet-Universal-Apps\DotnetUniversalApps\PerformanceImprovements\TextFile1.txt");

        Dictionary<string, int> frequency = new(StringComparer.OrdinalIgnoreCase);

        var lookup = frequency.GetAlternateLookup<ReadOnlySpan<char>>();

        for (int j = 0; j < 10; j++)
        {
            long mem = GC.GetTotalAllocatedBytes();
            stopwatch.Restart();

            for (int i = 0; i < 10; i++)
            {
                foreach (var m in Helpers.Whitespace().EnumerateSplits(text))
                {
                    var word = text.AsSpan(m);
                    lookup[word] = lookup.TryGetValue(word, out int count) ? count + 1 : 1;
                }
            }

            stopwatch.Stop();
            mem = GC.GetTotalAllocatedBytes() - mem;

            Console.WriteLine($"Time: {stopwatch.Elapsed.TotalMilliseconds}ms, Memory: {mem / 1024.0:N2} mb");
        }
    }

    public static async Task EnumerateSplitsWithCollectionMarshall()
    {
        Console.WriteLine("EnumerateSplitsWithCollectionMarshall");
        string text = await File.ReadAllTextAsync(@"D:\Programming\Projects\Roadmap\Dotnet-Universal-Apps\DotnetUniversalApps\PerformanceImprovements\TextFile1.txt");

        Dictionary<string, int> frequency = new(StringComparer.OrdinalIgnoreCase);

        var lookup = frequency.GetAlternateLookup<ReadOnlySpan<char>>();

        for (int j = 0; j < 10; j++)
        {
            long mem = GC.GetTotalAllocatedBytes();
            stopwatch.Restart();

            for (int i = 0; i < 10; i++)
            {
                foreach (var m in Helpers.Whitespace().EnumerateSplits(text))
                {
                    var word = text.AsSpan(m);
                    CollectionsMarshal.GetValueRefOrAddDefault(lookup, word, out _)++;
                }
            }

            stopwatch.Stop();
            mem = GC.GetTotalAllocatedBytes() - mem;

            Console.WriteLine($"Time: {stopwatch.Elapsed.TotalMilliseconds}ms, Memory: {mem / 1024.0:N2} mb");
        }
    }

    public static async Task SearchValuesExample()
    {
        Console.WriteLine("SearchValuesExample");
        string text = await File.ReadAllTextAsync(@"D:\Programming\Projects\Roadmap\Dotnet-Universal-Apps\DotnetUniversalApps\PerformanceImprovements\TextFile1.txt");

        string[] randArr = { "lorem", "ipsum", "dolor", "sit", "amet" };

        SearchValues<string> randArrSearchValues = SearchValues.Create(randArr, StringComparison.OrdinalIgnoreCase);

        for (int j = 0; j < 10; j++)
        {
            long mem = GC.GetTotalAllocatedBytes();
            stopwatch.Restart();

            for (int i = 0; i < 10; i++)
            {
                int count = 0;

                ReadOnlySpan<char> remaining = text;

                while (!remaining.IsEmpty)
                {
                    int pos = remaining.IndexOfAny(randArrSearchValues);

                    if (pos == -1)
                    {
                        break;
                    }

                    count++;
                    remaining = remaining[(pos + 1)..];
                }

                Helpers.Use(count);
            }

            stopwatch.Stop();
            mem = GC.GetTotalAllocatedBytes() - mem;

            Console.WriteLine($"Time: {stopwatch.Elapsed.TotalMilliseconds}ms, Memory: {mem / 1024.0:N2} mb");
        }
    }

    public static async Task SearchValuesExampleWithRegex()
    {
        Console.WriteLine("SearchValuesExampleWithRegex");
        string text = await File.ReadAllTextAsync(@"D:\Programming\Projects\Roadmap\Dotnet-Universal-Apps\DotnetUniversalApps\PerformanceImprovements\TextFile1.txt");

        for (int j = 0; j < 10; j++)
        {
            long mem = GC.GetTotalAllocatedBytes();
            stopwatch.Restart();

            for (int i = 0; i < 10; i++)
            {
                int count = Helpers.RandArr().Count(text);

                Helpers.Use(count);
            }

            stopwatch.Stop();
            mem = GC.GetTotalAllocatedBytes() - mem;

            Console.WriteLine($"Time: {stopwatch.Elapsed.TotalMilliseconds}ms, Memory: {mem / 1024.0:N2} mb");
        }
    }

    public static async Task TensorPrimitivesExample()
    {
        Console.WriteLine("TensorPrimitivesExample");

        byte[] bytes1 = new byte[1024];
        byte[] bytes2 = new byte[1024];

        Random.Shared.NextBytes(bytes1);
        Random.Shared.NextBytes(bytes2);

        for (int j = 0; j < 10; j++)
        {
            long mem = GC.GetTotalAllocatedBytes();
            stopwatch.Restart();

            for (int i = 0; i < 10; i++)
            {
                int distance = TensorPrimitives.HammingDistance<byte>(bytes1, bytes2);
            }

            stopwatch.Stop();
            mem = GC.GetTotalAllocatedBytes() - mem;

            Console.WriteLine($"Time: {stopwatch.Elapsed.TotalMilliseconds}ms, Memory: {mem / 1024.0:N2} mb");
        }
    }
}

public static partial class Helpers
{
    [GeneratedRegex(@"\b\w+\b")]
    public static partial Regex Words();

    [GeneratedRegex(@"\s+")]
    public static partial Regex Whitespace();

    [GeneratedRegex(@"lorem|ipsum|dolor|sit|amet", RegexOptions.IgnoreCase)]
    public static partial Regex RandArr();

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void Use<T>(T value)
    { }
}