namespace MassTransit.Transports
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Internals;
    using Middleware;
    using Util;


    public abstract class ConsumerAgent :
        Agent,
        DeliveryMetrics
    {
        readonly ReceiveEndpointContext _context;
        readonly TaskCompletionSource<bool> _deliveryComplete;
        readonly IReceivePipeDispatcher _dispatcher;
        readonly object _lock = new object();
        Task _consumeTask;
        TaskCompletionSource<bool> _consumeTaskSource;

        protected ConsumerAgent(ReceiveEndpointContext context)
        {
            _context = context;
            _deliveryComplete = TaskUtil.GetTask<bool>();

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

        protected void TrySetConsumeCompleted()
        {
            _consumeTaskSource?.TrySetResult(true);
        }

        protected void TrySetConsumeCanceled(CancellationToken cancellationToken = default)
        {
            if (_consumeTaskSource == null)
                return;

            if (IsIdle)
                _deliveryComplete.TrySetResult(false);

            _consumeTaskSource.TrySetCanceled(cancellationToken);
        }

        protected void TrySetConsumeException(Exception exception)
        {
            if (_consumeTaskSource == null)
                return;

            _deliveryComplete.TrySetResult(false);

            _consumeTaskSource.TrySetException(exception);
        }

        protected virtual async Task ActiveAndActualAgentsCompleted(StopContext context)
        {
            if (!IsIdle)
            {
                try
                {
                    await _deliveryComplete.Task.OrCanceled(context.CancellationToken).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    LogContext.Warning?.Log("Stop canceled waiting for message consumers to complete: {InputAddress}", _context.InputAddress);
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

        protected Task Dispatch(ReceiveContext context, ReceiveLockContext receiveLock = default)
        {
            return _dispatcher.Dispatch(context, receiveLock);
        }
    }
}
