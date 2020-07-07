namespace MassTransit.Util
{
    using System;
    using System.Threading;
    using System.Threading.Channels;
    using System.Threading.Tasks;


    public class ChannelExecutor :
        IAsyncDisposable
    {
        readonly Channel<IFuture> _channel;
        readonly Task[] _runTasks;
        readonly object _syncLock;

        public ChannelExecutor(int prefetchCount, int concurrencyLimit)
        {
            var channelOptions = new BoundedChannelOptions(prefetchCount)
            {
                AllowSynchronousContinuations = true,
                FullMode = BoundedChannelFullMode.Wait,
                SingleReader = concurrencyLimit == 1,
                SingleWriter = false
            };

            _channel = Channel.CreateBounded<IFuture>(channelOptions);

            _runTasks = new Task[concurrencyLimit];
            _syncLock = new object();

            for (var i = 0; i < concurrencyLimit; i++)
                _runTasks[i] = Task.Run(() => RunFromChannel());
        }

        public ChannelExecutor(int concurrencyLimit)
            :this (concurrencyLimit, true){ }

        public ChannelExecutor(int concurrencyLimit, bool allowSynchronousContinuations)
        {
            var channelOptions = new UnboundedChannelOptions
            {
                AllowSynchronousContinuations = allowSynchronousContinuations,
                SingleReader = concurrencyLimit == 1,
                SingleWriter = false
            };

            _channel = Channel.CreateUnbounded<IFuture>(channelOptions);

            _runTasks = new Task[concurrencyLimit];
            _syncLock = new object();

            for (var i = 0; i < concurrencyLimit; i++)
                _runTasks[i] = Task.Run(() => RunFromChannel());
        }

        public async ValueTask DisposeAsync()
        {
            _channel.Writer.Complete();

            await Task.WhenAll(_runTasks).ConfigureAwait(false);
        }

        public void PushWithWait(Func<Task> method, CancellationToken cancellationToken = default)
        {
            async Task<bool> RunMethod()
            {
                await method().ConfigureAwait(false);

                return true;
            }

            var future = new Future<bool>(() => RunMethod(), cancellationToken);


            while (!cancellationToken.IsCancellationRequested)
            {
                if (_channel.Writer.TryWrite(future))
                    return;

                lock (_syncLock)
                    Monitor.Wait(_syncLock, 1000);
            }

            cancellationToken.ThrowIfCancellationRequested();
        }

        public async Task Push(Func<Task> method, CancellationToken cancellationToken = default)
        {
            async Task<bool> RunMethod()
            {
                await method().ConfigureAwait(false);

                return true;
            }

            var future = new Future<bool>(() => RunMethod(), cancellationToken);

            await _channel.Writer.WriteAsync(future, cancellationToken).ConfigureAwait(false);
        }

        public Task Run(Func<Task> method, CancellationToken cancellationToken = default)
        {
            async Task<bool> RunMethod()
            {
                await method().ConfigureAwait(false);

                return true;
            }

            return Run(() => RunMethod(), cancellationToken);
        }

        public async Task<T> Run<T>(Func<Task<T>> method, CancellationToken cancellationToken = default)
        {
            var future = new Future<T>(method, cancellationToken);

            await _channel.Writer.WriteAsync(future, cancellationToken).ConfigureAwait(false);

            return await future.Completed.ConfigureAwait(false);
        }

        public Task Run(Action method, CancellationToken cancellationToken = default)
        {
            bool RunMethod()
            {
                method();

                return true;
            }

            return Run(() => RunMethod(), cancellationToken);
        }

        public async Task<T> Run<T>(Func<T> method, CancellationToken cancellationToken = default)
        {
            var future = new SynchronousFuture<T>(method, cancellationToken);

            await _channel.Writer.WriteAsync(future, cancellationToken).ConfigureAwait(false);

            return await future.Completed.ConfigureAwait(false);
        }

        async Task RunFromChannel()
        {
            while (await _channel.Reader.WaitToReadAsync().ConfigureAwait(false))
            {
                if (_channel.Reader.TryRead(out var future))
                {
                    var task = future.Run();

                    await task.ConfigureAwait(false);

                    lock (_syncLock)
                        Monitor.PulseAll(_syncLock);
                }
            }
        }


        interface IFuture
        {
            Task Run();
        }


        class Future<T> :
            IFuture
        {
            readonly CancellationToken _cancellationToken;
            readonly TaskCompletionSource<T> _completion;
            readonly Func<Task<T>> _method;

            public Future(Func<Task<T>> method, CancellationToken cancellationToken)
            {
                _method = method;
                _cancellationToken = cancellationToken;
                _completion = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);
            }

            /// <summary>
            /// The post-execution result, which can be awaited
            /// </summary>
            public Task<T> Completed => _completion.Task;

            public async Task Run()
            {
                if (_cancellationToken.IsCancellationRequested)
                {
                    _completion.TrySetCanceled();
                    return;
                }

                try
                {
                    var result = await _method().ConfigureAwait(false);

                    _completion.TrySetResult(result);
                }
                catch (OperationCanceledException exception) when (exception.CancellationToken == _cancellationToken)
                {
                    _completion.TrySetCanceled();
                }
                catch (Exception exception)
                {
                    _completion.TrySetException(exception);
                }
            }
        }


        class SynchronousFuture<T> :
            IFuture
        {
            readonly CancellationToken _cancellationToken;
            readonly TaskCompletionSource<T> _completion;
            readonly Func<T> _method;

            public SynchronousFuture(Func<T> method, CancellationToken cancellationToken)
            {
                _method = method;
                _cancellationToken = cancellationToken;
                _completion = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);
            }

            /// <summary>
            /// The post-execution result, which can be awaited
            /// </summary>
            public Task<T> Completed => _completion.Task;

            public Task Run()
            {
                if (_cancellationToken.IsCancellationRequested)
                {
                    _completion.TrySetCanceled();

                    return Task.CompletedTask;
                }

                try
                {
                    var result = _method();

                    _completion.TrySetResult(result);
                }
                catch (OperationCanceledException exception) when (exception.CancellationToken == _cancellationToken)
                {
                    _completion.TrySetCanceled();
                }
                catch (Exception exception)
                {
                    _completion.TrySetException(exception);
                }

                return Task.CompletedTask;
            }
        }
    }
}
