namespace MassTransit.Middleware.InMemoryOutbox
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Context;
    using MassTransit.Batching;
    using Util;


    public class InMemoryOutboxConsumeContext :
        ConsumeContextProxy,
        OutboxContext
    {
        readonly TaskCompletionSource<InMemoryOutboxConsumeContext> _clearToSend;
        readonly List<InMemoryOutboxMessageSchedulerContext> _outboxSchedulerContext;
        readonly List<Func<Task>> _pendingActions;

        public InMemoryOutboxConsumeContext(ConsumeContext context)
            : base(context)
        {
            CapturedContext = context;

            var outboxReceiveContext = new InMemoryOutboxReceiveContext(this, context.ReceiveContext);

            ReceiveContext = outboxReceiveContext;
            PublishEndpointProvider = outboxReceiveContext.PublishEndpointProvider;

            _pendingActions = new List<Func<Task>>();
            _clearToSend = TaskUtil.GetTask<InMemoryOutboxConsumeContext>();

            _outboxSchedulerContext = new List<InMemoryOutboxMessageSchedulerContext>();
            if (context.TryGetPayload(out MessageSchedulerContext schedulerContext))
            {
                var outboxSchedulerContext = (InMemoryOutboxMessageSchedulerContext)context.AddOrUpdatePayload<MessageSchedulerContext>(
                    () => new InMemoryOutboxMessageSchedulerContext(context, schedulerContext.SchedulerFactory, _clearToSend.Task),
                    existing => new InMemoryOutboxMessageSchedulerContext(context, existing.SchedulerFactory, _clearToSend.Task));

                _outboxSchedulerContext.Add(outboxSchedulerContext);
            }
        }

        public ConsumeContext CapturedContext { get; }

        public Task ClearToSend => _clearToSend.Task;

        public Task Add(Func<Task> method)
        {
            if (_clearToSend.Task.IsCompleted)
                return method();

            lock (_pendingActions)
            {
                _pendingActions.Add(method);

                return Task.CompletedTask;
            }
        }

        internal void AddChildSchedulerContext(InMemoryOutboxMessageSchedulerContext outboxSchedulerContext)
        {
            _outboxSchedulerContext.Add(outboxSchedulerContext);
        }

        public async Task ExecutePendingActions(bool concurrentMessageDelivery)
        {
            _clearToSend.TrySetResult(this);

            Func<Task>[] pendingActions;
            lock (_pendingActions)
                pendingActions = _pendingActions.ToArray();

            if (pendingActions.Length > 0)
            {
                if (concurrentMessageDelivery)
                {
                    var collection = new PendingTaskCollection(pendingActions.Length);

                    collection.Add(pendingActions.Select(action => action()));

                    await collection.Completed().ConfigureAwait(false);
                }
                else
                {
                    foreach (Func<Task> action in pendingActions)
                    {
                        var task = action();
                        if (task != null)
                            await task.ConfigureAwait(false);
                    }
                }
            }

            if (_outboxSchedulerContext != null)
            {
                try
                {
                    foreach (var outboxSchedulerContext in _outboxSchedulerContext)
                    {
                        await outboxSchedulerContext.ExecutePendingActions().ConfigureAwait(false);
                    }
                }
                catch (Exception e)
                {
                    LogContext.Warning?.Log(e, "One or more messages could not be unscheduled.", e);
                }
            }
        }

        public async Task DiscardPendingActions()
        {
            lock (_pendingActions)
                _pendingActions.Clear();

            if (_outboxSchedulerContext != null)
            {
                try
                {
                    foreach (var outboxSchedulerContext in _outboxSchedulerContext)
                    {
                        await outboxSchedulerContext.CancelAllScheduledMessages().ConfigureAwait(false);
                    }
                }
                catch (Exception e)
                {
                    LogContext.Warning?.Log(e, "One or more messages could not be unscheduled.", e);
                }
            }
        }
    }


    public class InMemoryOutboxConsumeContext<T> :
        InMemoryOutboxConsumeContext,
        ConsumeContext<T>
        where T : class
    {
        readonly ConsumeContext<T> _context;

        public InMemoryOutboxConsumeContext(ConsumeContext<T> context)
            : base(context)
        {
            _context = context;
        }

        public T Message => _context.Message;

        public virtual Task NotifyConsumed(TimeSpan duration, string consumerType)
        {
            return NotifyConsumed(this, duration, consumerType);
        }

        public virtual Task NotifyFaulted(TimeSpan duration, string consumerType, Exception exception)
        {
            return NotifyFaulted(this, duration, consumerType, exception);
        }

        public void Method1()
        {
        }

        public void Method2()
        {
        }

        public void Method3()
        {
        }
    }

    public class InMemoryOutboxBatchConsumeContext<T> :
        InMemoryOutboxConsumeContext,
        ConsumeContext<Batch<T>>
        where T : class
    {
        readonly MessageBatch<T> _batch;

        public InMemoryOutboxBatchConsumeContext(ConsumeContext<Batch<T>> context)
            : base(context)
        {
            var batch = context.Message;
            var messages = batch.Select(x => new InMemoryOutboxBatchMessageConsumeContext<T>(this, x)).ToList();
            _batch = new MessageBatch<T>(batch.FirstMessageReceived, batch.LastMessageReceived, batch.Mode, messages);
        }

        public Batch<T> Message => _batch;

        public virtual Task NotifyConsumed(TimeSpan duration, string consumerType)
        {
            return NotifyConsumed(this, duration, consumerType);
        }

        public virtual Task NotifyFaulted(TimeSpan duration, string consumerType, Exception exception)
        {
            return NotifyFaulted(this, duration, consumerType, exception);
        }

        public void Method1()
        {
        }

        public void Method2()
        {
        }

        public void Method3()
        {
        }
    }

    public class InMemoryOutboxBatchMessageConsumeContext<T> :
        ConsumeContextProxy<T>,
        OutboxContext
        where T : class
    {
        readonly InMemoryOutboxBatchConsumeContext<T> _batchContext;

        public InMemoryOutboxBatchMessageConsumeContext(
            InMemoryOutboxBatchConsumeContext<T> batchContext,
            ConsumeContext<T> context)
            : base(context)
        {
            _batchContext = batchContext;

            var outboxReceiveContext = new InMemoryOutboxReceiveContext(this, context.ReceiveContext);

            ReceiveContext = outboxReceiveContext;
            PublishEndpointProvider = outboxReceiveContext.PublishEndpointProvider;

            if (context.TryGetPayload(out MessageSchedulerContext schedulerContext))
            {
                var outboxSchedulerContext = (InMemoryOutboxMessageSchedulerContext)context.AddOrUpdatePayload<MessageSchedulerContext>(
                    () => new InMemoryOutboxMessageSchedulerContext(context, schedulerContext.SchedulerFactory, _batchContext.ClearToSend),
                    existing => new InMemoryOutboxMessageSchedulerContext(context, existing.SchedulerFactory, _batchContext.ClearToSend));

                batchContext.AddChildSchedulerContext(outboxSchedulerContext);
            }
        }

        public Task ClearToSend => _batchContext.ClearToSend;

        public Task Add(Func<Task> method) => _batchContext.Add(method);

        public Task DiscardPendingActions() => _batchContext.DiscardPendingActions();

        public Task ExecutePendingActions(bool concurrentMessageDelivery) => _batchContext.ExecutePendingActions(concurrentMessageDelivery);
    }
}
