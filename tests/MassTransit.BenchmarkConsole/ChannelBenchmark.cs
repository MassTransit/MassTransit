namespace MassTransit.BenchmarkConsole;

using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Util;


[SimpleJob(RuntimeMoniker.Net80)]
[MemoryDiagnoser]
[GcServer(true)]
[GcForce]
public class ChannelBenchmark :
    IAsyncDisposable
{
    readonly ChannelExecutor _singleChannel = new(1, 1);
    readonly TaskExecutor _taskExecutor = new();

    public ValueTask DisposeAsync()
    {
        return _singleChannel.DisposeAsync();
    }

    [Benchmark(Baseline = true, Description = "Regular Method")]
    public async Task RegularMethod()
    {
        await SubjectMethod().ConfigureAwait(false);
    }

    [Benchmark(Description = "Single Channel")]
    public async Task SingleChannelExecutor()
    {
        await _singleChannel.Run(SubjectMethod).ConfigureAwait(false);
    }

    [Benchmark(Description = "Super Channel")]
    public async Task SuperChannelExecutor()
    {
        await _taskExecutor.Run(SubjectMethod).ConfigureAwait(false);
    }

    static async Task SubjectMethod()
    {
        await SubjectChildMethod<long>().ConfigureAwait(false);
    }

    static async Task SubjectChildMethod<T>()
    {
    }
}


[SimpleJob(RuntimeMoniker.Net80)]
[MemoryDiagnoser]
[GcServer(true)]
[GcForce]
public class ConcurrentChannelBenchmark :
    IAsyncDisposable
{
    readonly ChannelExecutor _singleChannel = new(1, 10);
    readonly TaskExecutor _taskExecutor = new(10);

    public async ValueTask DisposeAsync()
    {
        await _singleChannel.DisposeAsync();

        await _taskExecutor.DisposeAsync();
    }

    [Benchmark(Baseline = true, Description = "Regular Method", OperationsPerInvoke = 10)]
    public async Task RegularMethod()
    {
        await Parallel.ForAsync(0, 10, async (n, token) => await SubjectMethod());
    }

    [Benchmark(Description = "Single Channel", OperationsPerInvoke = 10)]
    public async Task SingleChannelExecutor()
    {
        await Parallel.ForAsync(0, 10, async (n, token) => await _singleChannel.Run(SubjectMethod, token));
    }

    [Benchmark(Description = "Super Channel", OperationsPerInvoke = 10)]
    public async Task SuperChannelExecutor()
    {
        await Parallel.ForAsync(0, 10, async (n, token) => await _taskExecutor.Run(SubjectMethod, token));
    }

    // [Benchmark(Description = "Super Channel (Value)", OperationsPerInvoke = 10)]
    // public async Task SuperChannelExecutorValue()
    // {
    //     await Parallel.ForAsync(0, 10, (n, token) => _taskExecutor.Run(SubjectMethodValue, token));
    // }

    static async Task SubjectMethod()
    {
        await SubjectChildMethod<long>().ConfigureAwait(false);
    }

    static async Task SubjectChildMethod<T>()
    {
    }

    static async ValueTask SubjectMethodValue()
    {
        await SubjectChildMethodValue<long>().ConfigureAwait(false);
    }

    static async ValueTask SubjectChildMethodValue<T>()
    {
    }
}


public class MessageTypeChannelReader<T> :
    ChannelReader<ConsumeContext<T>>
    where T : class
{
    readonly ChannelReader<ConsumeContext> _source;

    public MessageTypeChannelReader(ChannelReader<ConsumeContext> source)
    {
        _source = source;
    }

    public override bool TryRead(out ConsumeContext<T> item)
    {
        while (_source.TryRead(out var read))
        {
            if (read.TryGetMessage(out ConsumeContext<T> messageContext))
            {
                item = messageContext;
                return true;
            }
        }

        item = null;
        return false;
    }

    public override ValueTask<bool> WaitToReadAsync(CancellationToken cancellationToken = new())
    {
        return _source.WaitToReadAsync(cancellationToken);
    }
}
