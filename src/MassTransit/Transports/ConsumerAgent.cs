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
        readonly ConcurrentDictionary<TKey, PendingReceiveLockContext> _pending;
        Task _consumeTask;
        TaskCompletionSource<bool> _consumeTaskSource;

        protected ConsumerAgent(ReceiveEndpointContext context, IEqualityComparer<TKey> equalityComparer = default)
        {
            _context = context;
            _deliveryComplete = TaskUtil.GetTask<bool>();

            _pending = new ConcurrentDictionary<TKey, PendingReceiveLockContext>(equalityComparer ?? EqualityComparer<TKey>.Default);

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
                _deliveryComplete.TrySetResult(true);

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

                    using var tokenSource = _context.StopTimeout.HasValue
                        ? new CancellationTokenSource(_context.StopTimeout.Value)
                        : new CancellationTokenSource();

                    await this.Stop("Consume Loop Exited", tokenSource.Token).ConfigureAwait(false);
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
            LogContext.Debug?.Log("Consumer Stopping: {InputAddress} ({Reason})", _context.InputAddress, context.Reason);

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
                    LogContext.Warning?.Log("Consumer stop canceled: {InputAddress}", _context.InputAddress);

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
                LogContext.Warning?.Log(e, "Consumer stop faulted: {InputAddress}", _context.InputAddress);
            }
        }

        protected virtual bool IsTrackable(TKey key)
        {
            return true;
        }

        protected Task Dispatch<TContext>(TKey key, TContext context, ReceiveLockContext receiveLockContext)
            where TContext : BaseReceiveContext
        {
            var added = false;
            var lockContext = receiveLockContext;

            if (IsTrackable(key))
            {
                lockContext = _pending.AddOrUpdate(key, _ =>
                {
                    var current = new PendingReceiveLockContext();
                    added = current.Enqueue(context, receiveLockContext);
                    return current;
                }, (_, current) =>
                {
                    added = current.Enqueue(context, receiveLockContext);
                    return current;
                });

                if (!added)
                {
                    context.LogTransportDupe(key);
                    return Task.CompletedTask;
                }
            }

            var dispatchTask = _dispatcher.Dispatch(context, lockContext);

            if (added)
            {
                dispatchTask.ContinueWith(_ =>
                {
                    if (_pending.TryGetValue(key, out var value))
                    {
                        if (value.IsEmpty)
                            _pending.TryRemove(key, out var _);
                    }
                }, TaskContinuationOptions.ExecuteSynchronously);
            }

            return dispatchTask;
        }
    }
}
