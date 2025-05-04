namespace MassTransit.InMemoryTransport;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Util;


public class InMemoryDelayProvider :
    IAsyncDisposable,
    IInMemoryDelayProvider
{
    readonly SortedList<DateTime, FutureDelay> _delays;
    readonly Channel<IOperation> _operations;
    readonly Task _readerTask;
    TimeSpan _nowOffset;

    public InMemoryDelayProvider()
    {
        _operations = Channel.CreateUnbounded<IOperation>(new UnboundedChannelOptions
        {
            AllowSynchronousContinuations = false,
            SingleReader = true,
            SingleWriter = false
        });

        _delays = new SortedList<DateTime, FutureDelay>(new DuplicateKeyComparer<DateTime>());
        _nowOffset = TimeSpan.Zero;

        _readerTask = Task.Run(() => DelayChannelReader());
    }

    public DateTime UtcNow => DateTime.UtcNow + _nowOffset;

    public async ValueTask DisposeAsync()
    {
        _operations.Writer.TryComplete();

        await _readerTask.ConfigureAwait(false);

        CancelPendingDelays();
    }

    public Task Delay(int milliseconds, CancellationToken cancellationToken = default)
    {
        return Delay(TimeSpan.FromMilliseconds(milliseconds), cancellationToken);
    }

    public Task Delay(TimeSpan delay, CancellationToken cancellationToken = default)
    {
        return Delay(UtcNow + delay, cancellationToken);
    }

    public async Task Delay(DateTime delayUntil, CancellationToken cancellationToken = default)
    {
        using var future = new FutureDelay(cancellationToken);

        var operation = new DelayOperation(delayUntil, future);

        await _operations.Writer.WriteAsync(operation, cancellationToken).ConfigureAwait(false);

        await future.DelayTask.ConfigureAwait(false);
    }

    public ValueTask Advance(TimeSpan duration)
    {
        if (duration <= TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(duration), "Must be greater than zero");

        return _operations.Writer.WriteAsync(new AdvanceTimeOperation(duration));
    }

    async Task DelayChannelReader()
    {
        while (_operations.Reader.Completion.IsCompleted == false)
        {
            var timeoutToken = CancellationToken.None;
            CancellationTokenSource timeoutSource = null;

            TimeSpan? timeout = GetDelayTimeout();
            if (timeout.HasValue)
            {
                timeoutSource = new CancellationTokenSource(timeout.Value);
                timeoutToken = timeoutSource.Token;
            }

            try
            {
                var operation = await _operations.Reader.ReadAsync(timeoutToken).ConfigureAwait(false);

                switch (operation)
                {
                    case AdvanceTimeOperation advance:
                        _nowOffset += advance.Duration;
                        break;
                    case DelayOperation delayOperation:
                        _delays.Add(delayOperation.Delay, delayOperation.Future);
                        break;
                }
            }
            catch (ChannelClosedException)
            {
                // nothing to see here
            }
            catch (OperationCanceledException)
            {
                //
            }
            catch (Exception exception)
            {
                LogContext.Warning?.Log(exception, "DelayChannelReader faulted");
            }
            finally
            {
                timeoutSource?.Dispose();
            }
        }
    }

    TimeSpan? GetDelayTimeout()
    {
        while (_delays.Count > 0)
        {
            var nextDelayTime = _delays.Keys[0];

            var delay = nextDelayTime - UtcNow;
            if (delay > TimeSpan.Zero)
                return delay;

            var nextDelay = _delays.Values[0];

            nextDelay.Complete();

            _delays.RemoveAt(0);
        }

        return null;
    }

    void CancelPendingDelays()
    {
        foreach (var delay in _delays.Values)
            delay.Cancel();

        _delays.Clear();
    }


    class DuplicateKeyComparer<TKey>
        : IComparer<TKey>
        where TKey : IComparable
    {
        public int Compare(TKey x, TKey y)
        {
            var result = x.CompareTo(y);

            return result == 0
                ? 1
                : result;
        }
    }


    class FutureDelay :
        IDisposable
    {
        readonly CancellationTokenRegistration _registration;
        readonly TaskCompletionSource<bool> _source;

        public FutureDelay(CancellationToken cancellationToken)
        {
            _source = TaskUtil.GetTask();

            if (cancellationToken.CanBeCanceled)
                _registration = cancellationToken.Register(() => _source.TrySetCanceled(cancellationToken));
        }

        public Task DelayTask => _source.Task;

        public void Dispose()
        {
            _registration.Dispose();
        }

        public void Complete()
        {
            _source.TrySetResult(true);
        }

        public void Cancel()
        {
            _source.TrySetCanceled();
        }
    }


    class AdvanceTimeOperation :
        IOperation
    {
        public AdvanceTimeOperation(TimeSpan duration)
        {
            Duration = duration;
        }

        public TimeSpan Duration { get; }
    }


    class DelayOperation :
        IOperation
    {
        public DelayOperation(DateTime delay, FutureDelay future)
        {
            Delay = delay;
            Future = future;
        }

        public DateTime Delay { get; }
        public FutureDelay Future { get; }
    }


    interface IOperation;
}
