namespace MassTransit.BenchmarkConsole;

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;


[SimpleJob(RuntimeMoniker.Net80)]
[MemoryDiagnoser]
[GcServer(true)]
[GcForce]
public class ConcurrencyBenchmark
{
    static Pending _pending = new();
    static long _id;

    readonly Dictionary<long, Pending> _dictionary = new();
    readonly ConcurrentDictionary<long, Pending> _concurrentDictionary = new();

    [Benchmark(Baseline = true, Description = "Dictionary (Lock)")]
    public async Task DictionaryWithLock()
    {
        var nextId = Interlocked.Increment(ref _id);
        lock (_dictionary)
            _dictionary.Add(nextId, _pending);

        lock (_dictionary)
            _dictionary.Remove(nextId);
    }

    [Benchmark(Description = "Dictionary (NoLock)")]
    public async Task DictionaryNoLock()
    {
        var nextId = Interlocked.Increment(ref _id);
        _dictionary.Add(nextId, _pending);

        _dictionary.Remove(nextId);
    }

    [Benchmark(Description = "ConcurrentDictionary")]
    public async Task ConcurrentDictionaryNoLock()
    {
        var nextId = Interlocked.Increment(ref _id);
        _concurrentDictionary.TryAdd(nextId, _pending);

        _concurrentDictionary.TryRemove(nextId, out _);
    }


    class Pending
    {
    }
}
