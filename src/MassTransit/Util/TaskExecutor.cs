namespace MassTransit.Util;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;


/// <summary>
/// The successor to <see cref="ChannelExecutor" />, now with a more optimized execution pipeline resulting in
/// lower memory usage and reduced overhead.
/// </summary>
public class TaskExecutor :
    IAsyncDisposable
{
    readonly Task _readerTask;
    readonly Channel<IFuture> _taskChannel;
    bool _disposed;

    public TaskExecutor(int concurrencyLimit = 1)
    {
        if (concurrencyLimit < 1)
            throw new ArgumentOutOfRangeException(nameof(concurrencyLimit), concurrencyLimit, "Must be >= 1");

        _taskChannel = Channel.CreateUnbounded<IFuture>(new UnboundedChannelOptions
        {
            AllowSynchronousContinuations = true,
            SingleReader = concurrencyLimit == 1,
            SingleWriter = false
        });

        _readerTask = concurrencyLimit == 1
            ? Task.Run(() => SingleReader())
            : Task.WhenAll(Enumerable.Range(0, concurrencyLimit).Select(_ => Task.Run(() => MultipleReader())));
    }

    public TaskExecutor(int prefetchCount, int concurrencyLimit = 1)
    {
        if (concurrencyLimit < 1)
            throw new ArgumentOutOfRangeException(nameof(concurrencyLimit), concurrencyLimit, "Must be >= 1");

        _taskChannel = Channel.CreateBounded<IFuture>(new BoundedChannelOptions(prefetchCount)
        {
            AllowSynchronousContinuations = true,
            SingleReader = concurrencyLimit == 1,
            SingleWriter = false
        });

        _readerTask = concurrencyLimit == 1
            ? Task.Run(() => SingleReader())
            : Task.WhenAll(Enumerable.Range(0, concurrencyLimit).Select(_ => Task.Run(() => MultipleReader())));
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
            return;

        _taskChannel.Writer.TryComplete();

        await _readerTask.ConfigureAwait(false);

        _disposed = true;
    }

    public async Task Run(Action method, CancellationToken cancellationToken = default)
    {
        var future = new ActionFuture(method, cancellationToken);

        await _taskChannel.Writer.WriteAsync(future, cancellationToken).ConfigureAwait(false);

        await future.Completed.ConfigureAwait(false);
    }

    public async Task Run(Func<Task> method, CancellationToken cancellationToken = default)
    {
        var future = new TaskFuture(method, cancellationToken);

        await _taskChannel.Writer.WriteAsync(future, cancellationToken).ConfigureAwait(false);

        await future.Completed.ConfigureAwait(false);
    }

    public async Task Push(Action method, CancellationToken cancellationToken = default)
    {
        var future = new ActionFuture(method, cancellationToken);

        await _taskChannel.Writer.WriteAsync(future, cancellationToken).ConfigureAwait(false);
    }

    public async Task Push(Func<Task> method, CancellationToken cancellationToken = default)
    {
        var future = new TaskFuture(method, cancellationToken);

        await _taskChannel.Writer.WriteAsync(future, cancellationToken).ConfigureAwait(false);
    }

    public async Task<T> Run<T>(Func<T> method, CancellationToken cancellationToken = default)
    {
        var future = new FuncFuture<T>(method, cancellationToken);

        await _taskChannel.Writer.WriteAsync(future, cancellationToken).ConfigureAwait(false);

        return await future.Completed.ConfigureAwait(false);
    }

    public async Task<T> Run<T>(Func<Task<T>> method, CancellationToken cancellationToken = default)
    {
        var future = new TaskFuture<T>(method, cancellationToken);

        await _taskChannel.Writer.WriteAsync(future, cancellationToken).ConfigureAwait(false);

        return await future.Completed.ConfigureAwait(false);
    }

    async Task MultipleReader()
    {
        try
        {
            while (await _taskChannel.Reader.WaitToReadAsync().ConfigureAwait(false))
            {
                if (!_taskChannel.Reader.TryRead(out var future))
                    continue;

                await future.Run().ConfigureAwait(false);
            }
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception exception)
        {
            LogContext.Warning?.Log(exception, "MultipleReader faulted");
        }
    }

    async Task SingleReader()
    {
        try
        {
            while (await _taskChannel.Reader.WaitToReadAsync().ConfigureAwait(false))
            {
                if (!_taskChannel.Reader.TryPeek(out var future))
                    continue;

                await future.Run().ConfigureAwait(false);

                await _taskChannel.Reader.ReadAsync().ConfigureAwait(false);
            }
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception exception)
        {
            LogContext.Warning?.Log(exception, "SingleReader faulted");
        }
    }


    interface IFuture
    {
        ValueTask Run();
    }


    class BaseFuture<T>
    {
        protected readonly CancellationToken CancellationToken;
        protected readonly TaskCompletionSource<T> Source;

        protected BaseFuture(CancellationToken cancellationToken)
        {
            CancellationToken = cancellationToken;

            Source = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);
        }

        protected bool IsCancellationRequested => CancellationToken.IsCancellationRequested;

        public Task<T> Completed => Source.Task;
    }


    class BaseFuture :
        BaseFuture<bool>
    {
        protected BaseFuture(CancellationToken cancellationToken)
            : base(cancellationToken)
        {
        }
    }


    class TaskFuture<T> :
        BaseFuture<T>,
        IFuture
    {
        readonly Func<Task<T>> _method;

        public TaskFuture(Func<Task<T>> method, CancellationToken cancellationToken)
            : base(cancellationToken)
        {
            _method = method;
        }

        public async ValueTask Run()
        {
            if (IsCancellationRequested)
            {
                Source.SetException(new OperationCanceledException(CancellationToken));
                return;
            }

            try
            {
                var result = await _method().ConfigureAwait(false);

                Source.SetResult(result);
            }
            catch (Exception exception)
            {
                Source.SetException(exception);
            }
        }
    }


    class TaskFuture :
        BaseFuture,
        IFuture
    {
        readonly Func<Task> _method;

        public TaskFuture(Func<Task> method, CancellationToken cancellationToken)
            : base(cancellationToken)
        {
            _method = method;
        }

        public async ValueTask Run()
        {
            if (IsCancellationRequested)
            {
                Source.SetException(new OperationCanceledException(CancellationToken));
                return;
            }

            try
            {
                await _method().ConfigureAwait(false);

                Source.SetResult(true);
            }
            catch (Exception exception)
            {
                Source.SetException(exception);
            }
        }
    }


    class FuncFuture<T> :
        BaseFuture<T>,
        IFuture
    {
        readonly Func<T> _method;

        public FuncFuture(Func<T> method, CancellationToken cancellationToken)
            : base(cancellationToken)
        {
            _method = method;
        }

        public async ValueTask Run()
        {
            if (IsCancellationRequested)
            {
                Source.SetException(new OperationCanceledException(CancellationToken));
                return;
            }

            try
            {
                var result = _method();

                Source.SetResult(result);
            }
            catch (Exception exception)
            {
                Source.SetException(exception);
            }
        }
    }


    class ActionFuture :
        BaseFuture,
        IFuture
    {
        readonly Action _method;

        public ActionFuture(Action method, CancellationToken cancellationToken)
            : base(cancellationToken)
        {
            _method = method;
        }

        public async ValueTask Run()
        {
            if (IsCancellationRequested)
            {
                Source.SetException(new OperationCanceledException(CancellationToken));
                return;
            }

            try
            {
                _method();

                Source.SetResult(true);
            }
            catch (Exception exception)
            {
                Source.SetException(exception);
            }
        }
    }
}
