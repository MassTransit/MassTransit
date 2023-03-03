namespace MassTransit.Transports
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Internals;
    using Middleware;
    using Util;


    public abstract class ConsumerAgent<TKey> :
        Agent,
        DeliveryMetrics
    {
        readonly ReceiveEndpointContext _context;
        readonly TaskCompletionSource<bool> _deliveryComplete;
        readonly IReceivePipeDispatcher _dispatcher;
        readonly object _lock = new object();
        readonly ConcurrentDictionary<TKey, BaseReceiveContext> _pending;
        Task _consumeTask;
        TaskCompletionSource<bool> _consumeTaskSource;

        protected ConsumerAgent(ReceiveEndpointContext context, IEqualityComparer<TKey> equalityComparer = default)
        {
            _context = context;
            _deliveryComplete = TaskUtil.GetTask<bool>();

            _pending = new ConcurrentDictionary<TKey, BaseReceiveContext>(equalityComparer);

            _dispatcher = context.CreateReceivePipeDispatcher();
            _dispatcher.ZeroActivity += HandleDeliveryComplete;
        }

        protected bool IsIdle => ActiveDispatchCount == 0;

        protected long ActiveDispatchCount => _dispatcher.ActiveDispatchCount;

        protected bool IsGracefulShutdown { get; private set; } = true;

        public long DeliveryCount => _dispatcher.DispatchCount;

        public int ConcurrentDeliveryCount => _dispatcher.MaxConcurrentDispatchCount;

        Task HandleDeliveryComplete()
        {
            if (IsStopping)
            {
                LogContext.Debug?.Log("Consumer shutdown completed: {InputAddress}", _context.InputAddress);

                _deliveryComplete.TrySetResult(true);
            }

            return Task.CompletedTask;
        }

        protected void TrySetManualConsumeTask()
        {
            if (_consumeTask != null || _consumeTaskSource != null)
                return;

            lock (_lock)
            {
                if (_consumeTask != null || _consumeTaskSource != null)
                    return;

                _consumeTaskSource = TaskUtil.GetTask<bool>();
                SetConsumeTask(_consumeTaskSource.Task);
            }
        }

        protected void TrySetConsumeTask(Task consumeTask)
        {
            if (_consumeTask != null)
                return;

            lock (_lock)
            {
                if (_consumeTask != null)
                    return;

                _consumeTask = consumeTask;
                SetConsumeTask(_consumeTask);
            }
        }

        void SetConsumeTask(Task consumeTask)
        {
            async Task TryStop()
            {
                if (IsStopping)
                    return;

                IsGracefulShutdown = false;

                try
                {
                    LogContext.SetCurrentIfNull(_context.LogContext);

                    await this.Stop("Consume Loop Exited").ConfigureAwait(false);
                }
                catch (Exception exception)
                {
                    LogContext.Warning?.Log(exception, "Stop Faulted");
                }
            }

            if (consumeTask.IsCompleted)
                Task.Run(() => TryStop());
            else
                consumeTask.ContinueWith(_ => TryStop(), TaskContinuationOptions.RunContinuationsAsynchronously);
        }

        protected override Task StopAgent(StopContext context)
        {
            LogContext.Debug?.Log("Stopping consumer: {InputAddress} with a reason: {Reason}", _context.InputAddress, context.Reason);

            TrySetConsumeCompleted();

            SetCompleted(ActiveAndActualAgentsCompleted(context));

            return Completed;
        }

        void CancelPendingConsumers()
        {
            foreach (var key in _pending.Keys)
            {
                if (_pending.TryRemove(key, out var context))
                    context.Cancel();
            }
        }

        protected void TrySetConsumeCompleted()
        {
            _consumeTaskSource?.TrySetResult(true);
        }

        protected void TrySetConsumeCanceled(CancellationToken cancellationToken = default)
        {
            if (_consumeTaskSource == null)
                return;

            CancelPendingConsumers();

            _consumeTaskSource.TrySetCanceled(cancellationToken);
        }

        protected void TrySetConsumeException(Exception exception)
        {
            if (_consumeTaskSource == null)
                return;

            CancelPendingConsumers();

            _consumeTaskSource.TrySetException(exception);
        }

        protected virtual async Task ActiveAndActualAgentsCompleted(StopContext context)
        {
            if (!IsIdle)
            {
                CancellationTokenSource cancellationTokenSource = null;
                CancellationTokenRegistration? registration = null;

                if (_context.ConsumerStopTimeout != null)
                {
                    cancellationTokenSource = new CancellationTokenSource(_context.ConsumerStopTimeout.Value);
                    registration = cancellationTokenSource.Token.Register(CancelPendingConsumers);
                }

                try
                {
                    await _deliveryComplete.Task.OrCanceled(context.CancellationToken).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    LogContext.Warning?.Log("Stop canceled waiting for message consumers to complete: {InputAddress}", _context.InputAddress);

                    CancelPendingConsumers();
                }
                finally
                {
                    registration?.Dispose();
                    cancellationTokenSource?.Dispose();
                }
            }

            if (_consumeTask == null)
                return;

            try
            {
                await _consumeTask.OrCanceled(context.CancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception e)
            {
                LogContext.Error?.Log(e, "Stop failed waiting for consume task to complete: {InputAddress}", _context.InputAddress);
            }
        }

        protected virtual bool IsTrackable(TKey key)
        {
            return true;
        }

        protected Task Dispatch<TContext>(TKey key, TContext context, ReceiveLockContext receiveLock)
            where TContext : BaseReceiveContext
        {
            var added = false;
            var lockContext = receiveLock;

            if (IsTrackable(key))
            {
                lockContext = _pending.AddOrUpdate(key, _ =>
                {
                    added = true;
                    context.Pending(receiveLock);
                    return context;
                }, (_, current) =>
                {
                    added = false;
                    current.Pending(receiveLock);
                    return current;
                });

                if (!added)
                {
                    LogContext.Warning?.Log("Duplicate dispatch key {Key}", key);
                    return Task.CompletedTask;
                }
            }

            async Task DispatchAsync()
            {
                try
                {
                    await _dispatcher.Dispatch(context, lockContext).ConfigureAwait(false);
                }
                finally
                {
                    if (added)
                        _pending.TryRemove(key, out _);
                }
            }

            return DispatchAsync();
        }
    }
}
